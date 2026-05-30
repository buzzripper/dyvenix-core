using System.Net;

namespace Dyvenix.Core.Exceptions;

public class HttpException : Exception
{
	public HttpException() : base() { }

	public HttpException(string message) : base(message) { }

	public HttpException(string message, Exception innerException) : base(message, innerException) { }

	public HttpException(HttpStatusCode statusCode)
	{
		StatusCode = statusCode;
	}

	public HttpException(HttpStatusCode statusCode, string message) : this(message)
	{
		StatusCode = statusCode;
	}

	public HttpStatusCode StatusCode { get; set; }
}
