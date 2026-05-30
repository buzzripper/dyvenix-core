namespace Dyvenix.Core.DTOs;

public class PingResult
{
	public PingResult()
	{
	}

	public PingResult(string module, string service)
	{
		Module = module;
		Service = service;
	}

	public string Module { get; set; } = null!;
	public string Service { get; set; } = null!;
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
