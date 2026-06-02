using Dyvenix.Core.ApiClients;
using Xunit;

namespace Dyvenix.Core.Tests.ApiClients;

public class MethodTypeTests
{
	[Fact]
	public void MethodType_DefinesExpectedMembers()
	{
		var names = Enum.GetNames<MethodType>();

		Assert.Contains(nameof(MethodType.Post), names);
		Assert.Contains(nameof(MethodType.Delete), names);
		Assert.Contains(nameof(MethodType.Put), names);
		Assert.Contains(nameof(MethodType.Patch), names);
		Assert.Equal(4, names.Length);
	}
}
