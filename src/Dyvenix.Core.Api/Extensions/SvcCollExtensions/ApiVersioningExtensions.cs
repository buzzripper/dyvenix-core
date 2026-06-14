using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Dyvenix.Core.Api.Extensions.SvcCollExtensions;

/// <summary>
/// Common extensions for API server hosts.
/// </summary>
public static class ApiVersioningExtensions
{
	/// <summary>
	/// Configures standard API versioning with default version 1.0.
	/// </summary>
	public static IServiceCollection AddStandardApiVersioning(this IServiceCollection services)
	{
		services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
		}).AddApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});

		return services;
	}

	///// <summary>
	///// Configures the standard API middleware pipeline.
	///// </summary>
	//public static WebApplication UseStandardApiPipeline(this WebApplication app)
	//{

	//	return app;
	//}
}
