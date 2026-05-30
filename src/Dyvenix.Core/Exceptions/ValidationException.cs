namespace Dyvenix.Core.Exceptions;

public class ValidationException : Exception
{
	public ValidationException(string message, Dictionary<string, string[]> fieldErrors)
		: base(message)
	{
		this.FieldErrors = fieldErrors;
	}

	public Dictionary<string, string[]>? FieldErrors;
}
