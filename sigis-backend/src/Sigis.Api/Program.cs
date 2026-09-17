using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Sigis.Application.UseCases.Auth;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure;
using Sigis.Infrastructure.Persistence.Context;
using Sigis.Infrastructure.Seed;

const string CorsPolicyName = "SigisCors";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext();

    if (context.HostingEnvironment.IsDevelopment())
    {
        configuration.WriteTo.Console();
    }
    else
    {
        configuration.WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter());
    }
});

builder.Services.AddControllers()
    // Sem isto, todo enum (SchoolStatus, ConsentType, DayOfWeek, etc.) é
    // serializado/desserializado como número em vez do nome — péssimo para
    // quem consome a API e impossível de adivinhar sem ler o código-fonte.
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Botão "Authorize" no Swagger: cole apenas o token, sem o prefixo "Bearer ".
    //
    // A partir do Microsoft.OpenApi 2.x (usado pelo Swashbuckle.AspNetCore
    // 10.x), a API mudou de AddSecurityDefinition/AddSecurityRequirement
    // para atribuição direta nas coleções de SwaggerGeneratorOptions, e a
    // referência ao esquema de segurança é resolvida contra o
    // OpenApiDocument sendo gerado (por isso o Func<OpenApiDocument, ...>).
    options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Cole o token JWT (sem 'Bearer ').",
    };

    options.SwaggerGeneratorOptions.SecurityRequirements.Add(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = [],
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

// Autenticação JWT — sem gateway, sem proxy: a própria API valida o token.
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Chave JWT não configurada ('Jwt:Secret').");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Sem isto, o handler remapeia claims curtas ("sub", "email", "role")
        // para os URIs longos do WS-Federation antes de aplicar
        // RoleClaimType/NameClaimType abaixo — o token emite exatamente
        // "sub"/"email"/"role" (ver JwtTokenService), então o remapeamento
        // padrão faz FindFirst("role") e RequireRole(...) nunca encontrarem a
        // claim, derrubando toda autorização baseada em papel (403 mesmo com
        // token válido) e ICurrentUserService.ProfessionalId (sempre null).
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero,
            // O token emite "role"/"unit_id" como claims curtas (ver
            // JwtTokenService) — sem isso, RequireRole() não encontraria a
            // claim (o mapeamento padrão espera o URI longo do WS-Federation).
            RoleClaimType = "role",
            NameClaimType = "name",
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("RequireCoordinator", policy => policy.RequireRole("Coordinator"));
    options.AddPolicy("RequireAuditor", policy => policy.RequireRole("Auditor"));
    options.AddPolicy("RequireCoordinatorOrAuditor", policy => policy.RequireRole("Coordinator", "Auditor"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var seedScope = app.Services.CreateScope();
    var dbContext = seedScope.ServiceProvider.GetRequiredService<SigisDbContext>();
    var passwordHasher = seedScope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DevelopmentSeeder.SeedAsync(dbContext, passwordHasher, CancellationToken.None);
}

app.UseSerilogRequestLogging();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapGet("/api/version", (IHostEnvironment env) => Results.Ok(new
{
    name = "SIGIS",
    version = "0.1.0",
    environment = env.EnvironmentName
}));

app.MapControllers();

app.Run();
