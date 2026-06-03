namespace Dyvenix.Core.Contracts
{
	public interface ITenantAccessService
	{
		Guid? TenantId { get; }
	}
}
