using FluentAssertions;
using Sigis.Domain.Abstractions;

namespace Sigis.Domain.Tests.Abstractions;

public class ResultTests
{
    private static readonly Error SampleError = new("TEST_001", "Erro de teste.");

    [Fact]
    public void Success_deve_criar_result_bem_sucedido_sem_erro()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_deve_criar_result_de_falha_com_erro()
    {
        var result = Result.Failure(SampleError);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SampleError);
    }

    [Fact]
    public void SuccessGenerico_deve_criar_resultT_bem_sucedido_com_valor()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void FailureGenerico_deve_criar_resultT_de_falha_com_erro()
    {
        var result = Result.Failure<int>(SampleError);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SampleError);
    }

    [Fact]
    public void Value_em_result_de_falha_deve_lancar_InvalidOperationException()
    {
        var result = Result<int>.Failure(SampleError);

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitOperator_deve_converter_valor_em_resultT_de_sucesso()
    {
        Result<string> result = "abc";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("abc");
    }

    [Fact]
    public void Match_em_result_generico_deve_executar_onSuccess_quando_sucesso()
    {
        var result = Result<int>.Success(10);

        var output = result.Match(
            onSuccess: value => $"sucesso:{value}",
            onFailure: error => $"falha:{error.Code}");

        output.Should().Be("sucesso:10");
    }

    [Fact]
    public void Match_em_result_generico_deve_executar_onFailure_quando_falha()
    {
        var result = Result<int>.Failure(SampleError);

        var output = result.Match(
            onSuccess: value => $"sucesso:{value}",
            onFailure: error => $"falha:{error.Code}");

        output.Should().Be("falha:TEST_001");
    }

    [Fact]
    public void Match_em_result_nao_generico_deve_executar_onSuccess_quando_sucesso()
    {
        var result = Result.Success();

        var output = result.Match(onSuccess: () => "ok", onFailure: error => error.Code);

        output.Should().Be("ok");
    }

    [Fact]
    public void Bind_deve_encadear_operacao_quando_sucesso()
    {
        var result = Result<int>.Success(2);

        var chained = result.Bind(value => Result<int>.Success(value * 10));

        chained.IsSuccess.Should().BeTrue();
        chained.Value.Should().Be(20);
    }

    [Fact]
    public void Bind_deve_propagar_falha_sem_executar_next()
    {
        var result = Result<int>.Failure(SampleError);
        var executed = false;

        var chained = result.Bind(value =>
        {
            executed = true;
            return Result<int>.Success(value * 10);
        });

        executed.Should().BeFalse();
        chained.IsFailure.Should().BeTrue();
        chained.Error.Should().Be(SampleError);
    }

    [Fact]
    public void Map_deve_transformar_valor_quando_sucesso()
    {
        var result = Result<int>.Success(3);

        var mapped = result.Map(value => value.ToString());

        mapped.Value.Should().Be("3");
    }

    [Fact]
    public void Ensure_deve_falhar_quando_predicado_nao_satisfeito()
    {
        var result = Result<int>.Success(3);

        var ensured = result.Ensure(value => value > 10, SampleError);

        ensured.IsFailure.Should().BeTrue();
        ensured.Error.Should().Be(SampleError);
    }

    [Fact]
    public void Ensure_deve_manter_sucesso_quando_predicado_satisfeito()
    {
        var result = Result<int>.Success(20);

        var ensured = result.Ensure(value => value > 10, SampleError);

        ensured.IsSuccess.Should().BeTrue();
        ensured.Value.Should().Be(20);
    }

    [Fact]
    public void ToResult_deve_encapsular_valor_em_sucesso()
    {
        var result = "valor".ToResult();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("valor");
    }

    [Fact]
    public void OnSuccess_deve_executar_acao_quando_sucesso()
    {
        var executions = 0;

        Result.Success().OnSuccess(() => executions++);

        executions.Should().Be(1);
    }

    [Fact]
    public void OnSuccess_nao_deve_executar_acao_quando_falha()
    {
        var executions = 0;

        Result.Failure(SampleError).OnSuccess(() => executions++);

        executions.Should().Be(0);
    }

    [Fact]
    public void OnFailure_deve_executar_acao_com_erro_quando_falha()
    {
        Error? capturedError = null;

        Result.Failure(SampleError).OnFailure(error => capturedError = error);

        capturedError.Should().Be(SampleError);
    }

    [Fact]
    public void OnFailure_nao_deve_executar_acao_quando_sucesso()
    {
        var executions = 0;

        Result.Success().OnFailure(_ => executions++);

        executions.Should().Be(0);
    }
}
