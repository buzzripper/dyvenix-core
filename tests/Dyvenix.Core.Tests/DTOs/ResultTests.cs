using Dyvenix.Core.DTOs;
using Dyvenix.Core.Exceptions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Dyvenix.Core.Tests.DTOs;

public class ResultTests
{
	[Fact]
	public void Ok_ReturnsSuccessWithNoError()
	{
		var result = Result.Ok();

		Assert.True(result.IsSuccess);
		Assert.Null(result.Error);
	}

	[Fact]
	public void NotFound_DefaultMessage_SetsNotFoundError()
	{
		var result = Result.NotFound();

		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);
		Assert.Equal(ResultErrorKind.NotFound, result.Error!.Kind);
		Assert.Equal("Resource not found", result.Error.Message);
	}

	[Fact]
	public void NotFound_CustomMessage_UsesProvidedMessage()
	{
		var result = Result.NotFound("missing");

		Assert.Equal("missing", result.Error!.Message);
	}

	[Fact]
	public void Validation_WithMessage_SetsValidationError()
	{
		var result = Result.Validation("bad input");

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultErrorKind.Validation, result.Error!.Kind);
		Assert.Equal("bad input", result.Error.Message);
	}

	[Fact]
	public void Validation_WithFieldErrors_PopulatesFieldErrors()
	{
		var fieldErrors = new Dictionary<string, string[]>
		{
			["Name"] = ["Required"]
		};

		var result = Result.Validation(fieldErrors);

		Assert.Equal(ResultErrorKind.Validation, result.Error!.Kind);
		Assert.Same(fieldErrors, result.Error.FieldErrors);
	}

	[Fact]
	public void Conflict_DefaultMessage_SetsConflictError()
	{
		var result = Result.Conflict();

		Assert.Equal(ResultErrorKind.Conflict, result.Error!.Kind);
		Assert.Equal("Resource conflict", result.Error.Message);
	}

	[Fact]
	public void Forbidden_DefaultMessage_SetsForbiddenError()
	{
		var result = Result.Forbidden();

		Assert.Equal(ResultErrorKind.Forbidden, result.Error!.Kind);
		Assert.Equal("Access denied", result.Error.Message);
	}

	[Fact]
	public void Failure_SetsFailureError()
	{
		var result = Result.Failure("boom");

		Assert.Equal(ResultErrorKind.Failure, result.Error!.Kind);
		Assert.Equal("boom", result.Error.Message);
	}

	[Fact]
	public void StatusCode_WhenSuccess_ReturnsFailureKindAsFallback()
	{
		// No error means Error?.Kind is null, falling back to ResultErrorKind.Failure (500).
		var result = Result.Ok();

		Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
	}

	[Theory]
	[InlineData(ResultErrorKind.Validation, HttpStatusCode.BadRequest)]
	[InlineData(ResultErrorKind.NotFound, HttpStatusCode.NotFound)]
	[InlineData(ResultErrorKind.Conflict, HttpStatusCode.Conflict)]
	[InlineData(ResultErrorKind.Failure, HttpStatusCode.InternalServerError)]
	public void StatusCode_MapsErrorKindToHttpStatusCode(ResultErrorKind kind, HttpStatusCode expected)
	{
		var result = kind switch
		{
			ResultErrorKind.Validation => Result.Validation("x"),
			ResultErrorKind.NotFound => Result.NotFound(),
			ResultErrorKind.Conflict => Result.Conflict(),
			_ => Result.Failure("x")
		};

		Assert.Equal(expected, result.StatusCode);
	}

	[Fact]
	public void ThrowIfFailure_WhenSuccess_DoesNotThrow()
	{
		var result = Result.Ok();

		var ex = Record.Exception(() => result.ThrowIfFailure());

		Assert.Null(ex);
	}

	[Fact]
	public void ThrowIfFailure_Validation_ThrowsValidationException()
	{
		var result = Result.Validation("invalid");

		var ex = Assert.Throws<ValidationException>(() => result.ThrowIfFailure());
		Assert.Equal("invalid", ex.Message);
	}

	[Fact]
	public void ThrowIfFailure_Forbidden_ThrowsForbiddenException()
	{
		var result = Result.Forbidden("no");

		Assert.Throws<ForbiddenException>(() => result.ThrowIfFailure());
	}

	[Fact]
	public void ThrowIfFailure_Conflict_ThrowsConcurrencyException()
	{
		var result = Result.Conflict("conflict");

		Assert.Throws<ConcurrencyException>(() => result.ThrowIfFailure());
	}

	[Fact]
	public void ThrowIfFailure_Failure_ThrowsException()
	{
		var result = Result.Failure("generic");

		var ex = Assert.Throws<Exception>(() => result.ThrowIfFailure());
		Assert.Equal("generic", ex.Message);
	}
}

public class ResultOfTTests
{
	[Fact]
	public void Ok_WithValue_ReturnsSuccessWithData()
	{
		var result = Result<int>.Ok(42);

		Assert.True(result.IsSuccess);
		Assert.Equal(42, result.Data);
		Assert.Null(result.Error);
	}

	[Fact]
	public void Failure_SetsErrorAndDefaultData()
	{
		var result = Result<string>.Failure("oops");

		Assert.False(result.IsSuccess);
		Assert.Null(result.Data);
		Assert.Equal(ResultErrorKind.Failure, result.Error!.Kind);
	}

	[Fact]
	public void Unauthorized_SetsUnauthorizedError()
	{
		var result = Result<string>.Unauthorized();

		Assert.Equal(ResultErrorKind.Unauthorized, result.Error!.Kind);
		Assert.Equal("Unauthorized", result.Error.Message);
	}

	[Fact]
	public void Match_WhenSuccess_InvokesOnSuccess()
	{
		var result = Result<int>.Ok(10);

		var output = result.Match(
			onSuccess: value => $"ok:{value}",
			onFailure: error => $"err:{error.Message}");

		Assert.Equal("ok:10", output);
	}

	[Fact]
	public void Match_WhenFailure_InvokesOnFailure()
	{
		var result = Result<int>.Validation("bad");

		var output = result.Match(
			onSuccess: value => $"ok:{value}",
			onFailure: error => $"err:{error.Message}");

		Assert.Equal("err:bad", output);
	}

	[Fact]
	public void Ok_RoundTripsThroughJsonSerialization()
	{
		var result = Result<string>.Ok("hello");

		var json = JsonSerializer.Serialize(result);
		var deserialized = JsonSerializer.Deserialize<Result<string>>(json);

		Assert.NotNull(deserialized);
		Assert.True(deserialized!.IsSuccess);
		Assert.Equal("hello", deserialized.Data);
	}
}

public class ResultErrorTests
{
	[Fact]
	public void ResultError_StoresKindMessageAndFieldErrors()
	{
		var fieldErrors = new Dictionary<string, string[]> { ["F"] = ["e"] };

		var error = new ResultError(ResultErrorKind.Validation, "msg", fieldErrors);

		Assert.Equal(ResultErrorKind.Validation, error.Kind);
		Assert.Equal("msg", error.Message);
		Assert.Same(fieldErrors, error.FieldErrors);
	}

	[Fact]
	public void ResultError_FieldErrorsDefaultsToNull()
	{
		var error = new ResultError(ResultErrorKind.Failure, "msg");

		Assert.Null(error.FieldErrors);
	}
}
