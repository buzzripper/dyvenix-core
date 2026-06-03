namespace Dyvenix.Core.Contracts;

public interface ICurrentUserService
{
	bool IsAuthenticated { get; }
	Guid? UserId { get; }
}
