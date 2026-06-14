using Microsoft.AspNetCore.Authorization;

namespace Dyvenix.Core.Api.Authorization;

public sealed class PermissionAuthorizationHandler(PermissionRegistry permissionRegistry)
	: AuthorizationHandler<PermissionRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
	{
		if (context.User?.Identity?.IsAuthenticated != true)
			return Task.CompletedTask;

		var rawPermissions = context.User.FindAll("perm")
			.SelectMany(claim => claim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

		// Expand permissions to include implied ones (e.g., auth:admin implies auth:write, auth:read)
		var permissions = permissionRegistry.ExpandPermissions(rawPermissions);

		if (permissions.Count == 0)
			return Task.CompletedTask;

		if (requirement.Permissions.Any(permission => permissions.Contains(permission)))
			context.Succeed(requirement);

		return Task.CompletedTask;
	}
}
