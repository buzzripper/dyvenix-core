using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dyvenix.Core.Api.Extensions.SvcCollExtensions;

public static class HealthExtensions
{
	// Add a default liveness check to ensure app is responsive
	public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services)
	{
		services.AddHealthChecks().AddCheck(Constants.AlivenessEndpointName, () => HealthCheckResult.Healthy(), new[] { Constants.AlivenessEndpointName });

		return services;
	}
}
