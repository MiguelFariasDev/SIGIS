FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.sln .
COPY src/Sigis.Domain/*.csproj src/Sigis.Domain/
COPY src/Sigis.Application/*.csproj src/Sigis.Application/
COPY src/Sigis.Infrastructure/*.csproj src/Sigis.Infrastructure/
COPY src/Sigis.Api/*.csproj src/Sigis.Api/
COPY src/Sigis.Mobile.Contracts/*.csproj src/Sigis.Mobile.Contracts/
RUN dotnet restore
COPY . .
RUN dotnet publish src/Sigis.Api/Sigis.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Sigis.Api.dll"]
