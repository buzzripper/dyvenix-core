using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;

namespace Dyvenix.Core.Api.Extensions.BuilderExtensions;

public static class TelemetryExtensions
{
	/// <summary>
	/// Configures OpenTelemetry for logging with OTLP export.
	/// </summary>
	public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	{
		// Configure OpenTelemetry logging
		builder.Logging.AddOpenTelemetry(logging =>
		{
			logging.IncludeFormattedMessage = true;
			logging.IncludeScopes = true;
		});

		// Add OTLP exporter if endpoint is configured (Aspire Dashboard or external collector)
		if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
			builder.Services.AddOpenTelemetry().UseOtlpExporter();

		return builder;
	}

}
