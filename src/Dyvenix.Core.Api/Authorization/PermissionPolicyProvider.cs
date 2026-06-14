using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Dyvenix.Core.Api.Authorization;

public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
	private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

	public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
	{
		_fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
	}

	public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
	{
		if (string.IsNullOrWhiteSpace(policyName))
			return _fallbackPolicyProvider.GetPolicyAsync(policyName);

		var permissions = policyName.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (permissions.Length == 0)
			return _fallbackPolicyProvider.GetPolicyAsync(policyName);

		var policy = new AuthorizationPolicyBuilder()
			.AddRequirements(new PermissionRequirement(permissions))
			.Build();

		return Task.FromResult<AuthorizationPolicy?>(policy);
	}

	public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

	public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();
}
