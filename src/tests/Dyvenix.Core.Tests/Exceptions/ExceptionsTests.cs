using Dyvenix.Core.Exceptions;
using Xunit;

namespace Dyvenix.Core.Tests.Exceptions;

public class ExceptionsTests
{
	[Fact]
	public void ValidationException_StoresMessageAndFieldErrors()
	{
		var fieldErrors = new Dictionary<string, string[]> { ["Name"] = ["Required"] };

		var ex = new ValidationException("Validation failed", fieldErrors);

		Assert.Equal("Validation failed", ex.Message);
		Assert.Same(fieldErrors, ex.FieldErrors);
	}

	[Fact]
	public void HttpException_WithStatusCodeAndMessage_StoresBoth()
	{
		var ex = new HttpException(System.Net.HttpStatusCode.NotFound, "not found");

		Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
		Assert.Equal("not found", ex.Message);
	}

	[Fact]
	public void HttpException_WithStatusCodeOnly_StoresStatusCode()
	{
		var ex = new HttpException(System.Net.HttpStatusCode.BadGateway);

		Assert.Equal(System.Net.HttpStatusCode.BadGateway, ex.StatusCode);
	}

	[Fact]
	public void HttpException_WithInnerException_StoresInnerException()
	{
		var inner = new InvalidOperationException("inner");

		var ex = new HttpException("outer", inner);

		Assert.Equal("outer", ex.Message);
		Assert.Same(inner, ex.InnerException);
	}

	public static IEnumerable<object[]> SimpleExceptionFactories()
	{
		yield return [(Func<string, Exception>)(m => new HttpDeserializationException(m))];
		yield return [(Func<string, Exception>)(m => new ConcurrencyException(m))];
		yield return [(Func<string, Exception>)(m => new ForbiddenException(m))];
		yield return [(Func<string, Exception>)(m => new NotFoundException(m))];
		yield return [(Func<string, Exception>)(m => new UnauthorizedException(m))];
	}

	[Theory]
	[MemberData(nameof(SimpleExceptionFactories))]
	public void SimpleException_WithMessage_StoresMessage(Func<string, Exception> factory)
	{
		var ex = factory("something went wrong");

		Assert.Equal("something went wrong", ex.Message);
	}

	[Fact]
	public void SimpleExceptions_AreOfTypeException()
	{
		Assert.IsAssignableFrom<Exception>(new ConcurrencyException());
		Assert.IsAssignableFrom<Exception>(new ForbiddenException());
		Assert.IsAssignableFrom<Exception>(new NotFoundException());
		Assert.IsAssignableFrom<Exception>(new UnauthorizedException());
		Assert.IsAssignableFrom<Exception>(new HttpDeserializationException());
	}

	[Fact]
	public void ConcurrencyException_WithInnerException_StoresInnerException()
	{
		var inner = new Exception("inner");

		var ex = new ConcurrencyException("outer", inner);

		Assert.Same(inner, ex.InnerException);
	}
}
