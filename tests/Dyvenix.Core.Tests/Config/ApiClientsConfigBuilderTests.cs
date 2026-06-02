using Dyvenix.Core.Config;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Dyvenix.Core.Tests.Config;

public class ApiClientsConfigBuilderTests
{
	private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
	{
		return new ConfigurationBuilder()
			.AddInMemoryCollection(values)
			.Build();
	}

	[Fact]
	public void Build_BindsNamedClientsFromConfiguration()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["ApiClients:OrdersApi:BaseUrl"] = "https://orders.local",
			["ApiClients:OrdersApi:TimeoutSecs"] = "30",
			["ApiClients:UsersApi:BaseUrl"] = "https://users.local",
			["ApiClients:UsersApi:TimeoutSecs"] = "15"
		});

		var config = ApiClientsConfigBuilder.Build(configuration);

		Assert.Equal(2, config.Count);
		Assert.Equal("https://orders.local", config["OrdersApi"].BaseUrl);
		Assert.Equal(30, config["OrdersApi"].TimeoutSecs);
		Assert.Equal("https://users.local", config["UsersApi"].BaseUrl);
		Assert.Equal(15, config["UsersApi"].TimeoutSecs);
	}

	[Fact]
	public void Build_MissingSection_ThrowsApplicationException()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["SomethingElse:Key"] = "value"
		});

		var ex = Assert.Throws<ApplicationException>(() => ApiClientsConfigBuilder.Build(configuration));
		Assert.Contains("ApiClients", ex.Message);
	}
}

public class ApiClientConfigTests
{
	[Fact]
	public void ApiClientConfig_StoresBaseUrlAndTimeout()
	{
		var config = new ApiClientConfig
		{
			BaseUrl = "https://api.local",
			TimeoutSecs = 60
		};

		Assert.Equal("https://api.local", config.BaseUrl);
		Assert.Equal(60, config.TimeoutSecs);
	}

	[Fact]
	public void ApiClientsConfig_IsDictionaryOfClientConfigs()
	{
		var config = new ApiClientsConfig
		{
			["Primary"] = new ApiClientConfig { BaseUrl = "https://primary.local", TimeoutSecs = 10 }
		};

		Assert.Single(config);
		Assert.Equal("https://primary.local", config["Primary"].BaseUrl);
	}
}
