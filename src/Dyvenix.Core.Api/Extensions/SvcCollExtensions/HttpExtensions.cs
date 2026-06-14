using Dyvenix.Core.Contracts;
using Dyvenix.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dyvenix.Core.Api.Extensions.SvcCollExtensions;

public static class HttpExtensions
{
	public static IServiceCollection AddCurrentUserServices(this IServiceCollection services)
	{
		services.ConfigureHttpClientDefaults(http =>
		{
			// Turn on resilience by default
			http.AddStandardResilienceHandler();

			// Turn on service discovery by default
			http.AddServiceDiscovery();
		});

		services.AddHttpContextAccessor();
		services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();

		return services;
	}
}
