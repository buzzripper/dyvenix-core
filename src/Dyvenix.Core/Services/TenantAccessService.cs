using Dyvenix.Core.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dyvenix.Core.Services
{
	public class TenantAccessService : ITenantAccessService
	{
		private readonly ClaimsPrincipal? _claimsPrincipal;
		private Guid? _tenantId;

		public TenantAccessService(IHttpContextAccessor httpContextAccessor)
		{
			_claimsPrincipal = httpContextAccessor.HttpContext?.User;
		}

		public Guid? TenantId
		{
			get
			{
				if (_tenantId != null)
					return _tenantId;

				var tenantIdStr = _claimsPrincipal?.FindFirst(GlobalConstants.ClaimName_TenantId)?.Value;
				if (tenantIdStr != null)
				{
					if (Guid.TryParse(tenantIdStr, out var tenantId))
					{
						if (tenantId != Guid.Empty)
							_tenantId = tenantId;
						else
							_tenantId = null;
					}
					else
					{
						_tenantId = null;
					}
				}

				return _tenantId;
			}
		}
	}
}
