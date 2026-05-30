namespace Dyvenix.Core.Config;

public class ApiClientsConfig : Dictionary<string, ApiClientConfig>
{
}

public class ApiClientConfig
{
	public required string BaseUrl { get; set; }
	public int TimeoutSecs { get; set; }
}
