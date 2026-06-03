using Dyvenix.Core.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dyvenix.Core.Services;

public sealed class HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
	public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

	public Guid? UserId
	{
		get
		{
			if (!IsAuthenticated)
				return null;

			var user = httpContextAccessor.HttpContext?.User;
			var userIdValue = user?.FindFirst("sub")?.Value
				?? user?.FindFirst("uid")?.Value
				?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			return Guid.TryParse(userIdValue, out var userId)
				? userId
				: null;
		}
	}
}
