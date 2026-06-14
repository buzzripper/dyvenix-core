using Dyvenix.Core.Api.Authorization;
using Dyvenix.Core.Contracts;
using Dyvenix.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dyvenix.Core.Api.Extensions.SvcCollExtensions;

public static class AuthExtensions
{
	public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
	{
		services.AddAuthorization();
		services.AddSingleton<PermissionRegistry>();
		services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
		services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

		return services;
	}

	public static IServiceCollection AddTestJwtAuthentication(this IServiceCollection services)
	{
		services.AddAuthentication(TestJwtAuthenticationHandler.SchemeName)
			.AddScheme<AuthenticationSchemeOptions, TestJwtAuthenticationHandler>(
				TestJwtAuthenticationHandler.SchemeName, _ => { });

		return services;
	}

	/// <summary>
	/// Configures JWT Bearer authentication using the OpenIddict authority.
	/// Reads settings from the "Authentication" configuration section.
	/// </summary>
	public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.Authority = configuration["Authentication:Authority"];
				options.Audience = configuration["Authentication:Audience"];

				options.TokenValidationParameters.NameClaimType = "name";
				options.TokenValidationParameters.RoleClaimType = "role";
			});

		services.AddAuthorization();

		services.AddScoped<ITenantAccessService, TenantAccessService>();

		return services;
	}
}
