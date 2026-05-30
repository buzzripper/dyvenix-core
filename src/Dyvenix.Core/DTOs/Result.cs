using Dyvenix.Core.Exceptions;
using System.Net;
using System.Text.Json.Serialization;

namespace Dyvenix.Core.DTOs;

/// <summary>
/// Represents the outcome of an operation that can succeed (no value) or fail with an error.
/// </summary>
public class Result
{
	[JsonConstructor]
	protected Result() { }

	[JsonInclude]
	public bool IsSuccess { get; protected init; }
	[JsonInclude]
	public ResultError? Error { get; protected init; }
	public HttpStatusCode StatusCode => (HttpStatusCode)(Error?.Kind ?? ResultErrorKind.Failure);

	public static Result Ok() => new() { IsSuccess = true };

	public static Result NotFound(string? message = null) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.NotFound, message ?? "Resource not found")
	};

	public static Result Validation(string message) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Validation, message)
	};

	public static Result Validation(Dictionary<string, string[]> fieldErrors) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Validation, "Validation failed", fieldErrors)
	};

	public static Result Conflict(string? message = null) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Conflict, message ?? "Resource conflict")
	};

	public static Result Unauthorized(string? message = null) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Forbidden, message ?? "Access denied")
	};

	public static Result Forbidden(string? message = null) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Forbidden, message ?? "Access denied")
	};

	public static Result Failure(string message) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Failure, message)
	};

	public void ThrowIfFailure()
	{
		if (!IsSuccess)
		{
			switch (Error!.Kind)
			{
				case ResultErrorKind.Validation:
					throw new ValidationException(Error.Message, Error?.FieldErrors ?? []);
				case ResultErrorKind.Unauthorized:
					throw new UnauthorizedException(Error.Message);
				case ResultErrorKind.Forbidden:
					throw new ForbiddenException(Error.Message);
				case ResultErrorKind.NotFound:
					throw new ArgumentNullException(Error.Message);
				case ResultErrorKind.Conflict:
					throw new ConcurrencyException(Error.Message);
				default:
					throw new Exception(Error.Message);
			}
		}
	}
}

/// <summary>
/// Represents the outcome of an operation that can succeed with a value or fail with an error.
/// </summary>
public class Result<T> : Result
{
	[JsonConstructor]
	private Result() { }

	[JsonInclude]
	public T? Data { get; private init; }

	public static Result<T> Ok(T value) => new()
	{
		IsSuccess = true,
		Data = value
	};

	public static new Result<T> Validation(string message) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Validation, message)
	};

	public static new Result<T> Validation(Dictionary<string, string[]> fieldErrors) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Validation, "Validation failed", fieldErrors)
	};

	public static new Result<T> Unauthorized(string? message = null) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Unauthorized, message ?? "Unauthorized")
	};

	public static new Result<T> Forbidden(string? message = null) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Forbidden, message ?? "Access denied")
	};

	public static new Result<T> NotFound(string? message = null) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.NotFound, message ?? "Resource not found")
	};

	public static new Result<T> Conflict(string? message = null) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Conflict, message ?? "Resource conflict")
	};

	public static new Result<T> Failure(string message) => new()
	{
		IsSuccess = false,
		Error = new(ResultErrorKind.Failure, message)
	};

	/// <summary>
	/// Pattern-match on success/failure.
	/// </summary>
	public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<ResultError, TOut> onFailure)
		=> IsSuccess ? onSuccess(Data!) : onFailure(Error!);
}

public enum ResultErrorKind
{
	Validation = 400,
	Unauthorized = 401,
	Forbidden = 403,
	NotFound = 404,
	Conflict = 409,
	Failure = 500
}

public record ResultError(
	ResultErrorKind Kind,
	string Message,
	Dictionary<string, string[]>? FieldErrors = null!
);
