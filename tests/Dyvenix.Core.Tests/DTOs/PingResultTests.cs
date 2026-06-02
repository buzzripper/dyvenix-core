using Dyvenix.Core.DTOs;
using Xunit;

namespace Dyvenix.Core.Tests.DTOs;

public class PingResultTests
{
	[Fact]
	public void DefaultConstructor_SetsTimestampToUtcNow()
	{
		var before = DateTime.UtcNow;

		var result = new PingResult();

		var after = DateTime.UtcNow;
		Assert.InRange(result.Timestamp, before, after);
	}

	[Fact]
	public void ParameterizedConstructor_SetsModuleAndService()
	{
		var result = new PingResult("Billing", "InvoiceService");

		Assert.Equal("Billing", result.Module);
		Assert.Equal("InvoiceService", result.Service);
	}

	[Fact]
	public void Properties_AreMutable()
	{
		var timestamp = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		var result = new PingResult
		{
			Module = "M",
			Service = "S",
			Timestamp = timestamp
		};

		Assert.Equal("M", result.Module);
		Assert.Equal("S", result.Service);
		Assert.Equal(timestamp, result.Timestamp);
	}
}
