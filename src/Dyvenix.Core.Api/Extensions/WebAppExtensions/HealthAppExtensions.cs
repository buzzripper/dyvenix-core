using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Dyvenix.Core.Api.Extensions.WebAppExtensions;

/// <summary>
/// Common extensions for resilience, health checks, and OpenTelemetry.
/// This project should be referenced by each service project in your solution.
/// Based on Aspire ServiceDefaults pattern.
/// </summary>
public static class HealthAppExtensions
{
	///// <summary>
	///// Adds service defaults including OpenTelemetry, health checks, service discovery, and HTTP client resilience.
	// </summary>
	//public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	//{
	//	//builder.ConfigureOpenTelemetry();

	//	//builder.AddDefaultHealthChecks();

	//	//builder.Services.AddServiceDiscovery();

	//	return builder;
	//}

	///// <summary>
	///// Configures OpenTelemetry for logging with OTLP export.
	///// </summary>
	//public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	//{
	//	// Configure OpenTelemetry logging
	//	builder.Logging.AddOpenTelemetry(logging =>
	//	{
	//		logging.IncludeFormattedMessage = true;
	//		logging.IncludeScopes = true;
	//	});

	//	// Add OTLP exporter if endpoint is configured (Aspire Dashboard or external collector)
	//	if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
	//		builder.Services.AddOpenTelemetry().UseOtlpExporter();

	//	return builder;
	//}

	//// Add a default liveness check to ensure app is responsive
	//public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	//{
	//	builder.Services.AddHealthChecks()
	//		.AddCheck(AlivenessEndpointName, () => HealthCheckResult.Healthy(), [AlivenessEndpointName]);

	//	return builder;
	//}

	/// <summary>
	/// Maps health check endpoints for readiness and liveness probes.
	/// </summary>
	public static WebApplication MapHealthEndpoints(this WebApplication app)
	{
		// Only health checks tagged with the "live" tag must pass for app to be considered alive
		app.MapHealthChecks($"/{Constants.AlivenessEndpointName}", new HealthCheckOptions
		{
			Predicate = r => r.Tags.Contains(Constants.AlivenessEndpointName)
		});

		// All health checks must pass for app to be considered ready to accept traffic after starting
		app.MapHealthChecks($"/{Constants.HealthEndpointName}", new HealthCheckOptions
		{
			ResponseWriter = async (context, report) =>
			{
				context.Response.ContentType = "application/json";

				var result = new
				{
					status = report.Status.ToString(),
					checks = report.Entries.Select(e => new
					{
						name = e.Key,
						status = e.Value.Status.ToString(),
						description = e.Value.Description,
						data = e.Value.Data   // <-- THIS is your dictionary
					})
				};

				await context.Response.WriteAsync(JsonSerializer.Serialize(result));
			}
		});

		return app;
	}
}
