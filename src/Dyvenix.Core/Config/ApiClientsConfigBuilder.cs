using Microsoft.Extensions.Configuration;

namespace Dyvenix.Core.Config;

public static class ApiClientsConfigBuilder
{
	private const string cConfigSectionName = "ApiClients";

	public static ApiClientsConfig Build(IConfiguration configuration)
	{
		var config = configuration.GetSection(cConfigSectionName).Get<ApiClientsConfig>();
		if (config == null)
			throw new ApplicationException($"Unable to retrieve {cConfigSectionName} section from appsettings.json file.");

		return config;
	}
}
