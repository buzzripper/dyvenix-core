using Dyvenix.Core.DTOs;
using Dyvenix.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Dyvenix.Core.Api.Handlers;

public sealed class ApiExceptionHandler : IExceptionHandler
{
	private readonly ILoggerFactory _loggerFactory;

	public ApiExceptionHandler(ILoggerFactory loggerFactory)
	{
		_loggerFactory = loggerFactory;
	}

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		// Minimal APIs have no controller — the endpoint's handler MethodInfo
		// is the equivalent "source class + method".
		var methodInfo = httpContext.GetEndpoint()?.Metadata.GetMetadata<MethodInfo>();
		var sourceType = methodInfo?.DeclaringType;          // e.g. AppRegistrationEndpoints
		var sourceClass = sourceType?.Name ?? "Unknown";
		var sourceMethod = methodInfo?.Name ?? httpContext.Request.Path.ToString();

		// Same category you'd get from ILogger<T>, chosen per-endpoint at runtime.
		var logger = _loggerFactory.CreateLogger(sourceType ?? typeof(ApiExceptionHandler));
		logger.LogError(exception, "Unhandled exception in {Source}.{Method}", sourceClass, sourceMethod);

		var result = MapExceptionToResult(exception);

		httpContext.Response.StatusCode = (int)result.StatusCode;
		await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

		return true; // handled — stop the pipeline
	}

	private static Result MapExceptionToResult(Exception ex) => ex switch
	{
		ValidationException => Result.Validation(ex.GetBaseException().Message),
		UnauthorizedException => Result.Unauthorized(ex.GetBaseException().Message),
		ForbiddenException => Result.Forbidden(ex.GetBaseException().Message),
		NotFoundException => Result.NotFound(ex.GetBaseException().Message),
		ConcurrencyException => Result.Conflict(ex.GetBaseException().Message),
		_ => Result.Failure(ex.GetBaseException().Message)
	};
}