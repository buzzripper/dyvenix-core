using Dyvenix.Core.DTOs;
using Xunit;

namespace Dyvenix.Core.Tests.DTOs;

public class ListPageTests
{
	[Fact]
	public void DefaultConstructor_CreatesEmptyList()
	{
		var page = new ListPage<string>();

		Assert.NotNull(page.Items);
		Assert.Empty(page.Items);
		Assert.Equal(0, page.TotalRowCount);
	}

	[Fact]
	public void EnumerableConstructor_PopulatesItems()
	{
		var source = new[] { "a", "b", "c" };

		var page = new ListPage<string>(source);

		Assert.Equal(source, page.Items);
	}

	[Fact]
	public void EnumerableConstructor_DoesNotSetTotalRowCount()
	{
		var page = new ListPage<int>(new[] { 1, 2 });

		Assert.Equal(0, page.TotalRowCount);
	}

	[Fact]
	public void TotalRowCount_CanDifferFromItemCount()
	{
		var page = new ListPage<int>(new[] { 1, 2 })
		{
			TotalRowCount = 100
		};

		Assert.Equal(2, page.Items.Count);
		Assert.Equal(100, page.TotalRowCount);
	}
}
