using Dyvenix.Core.DTOs;
using Xunit;

namespace Dyvenix.Core.Tests.DTOs;

public class StatusLevelTests
{
	[Theory]
	[InlineData(StatusLevel.Ok, 0)]
	[InlineData(StatusLevel.Info, 1)]
	[InlineData(StatusLevel.Warning, 2)]
	[InlineData(StatusLevel.Error, 3)]
	[InlineData(StatusLevel.Critical, 4)]
	public void StatusLevel_HasExpectedOrdinalValues(StatusLevel level, int expected)
	{
		Assert.Equal(expected, (int)level);
	}

	[Fact]
	public void StatusLevel_DefinesExactlyFiveValues()
	{
		var values = Enum.GetValues<StatusLevel>();

		Assert.Equal(5, values.Length);
	}
}
