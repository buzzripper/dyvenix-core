namespace Dyvenix.Core.DTOs;

/// <summary>
/// Holds a list of items for returning from a paging request, includes the total row count (not the count of items in the list),
/// for calculating the number of pages. This is used for paging requests where the client needs to know how many total rows there are 
/// in order to calculate the number of pages, but only a subset of the rows are returned in the list.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListPage<T>
{
	public ListPage()
	{ }

	public ListPage(IEnumerable<T> entityList)
	{
		Items.AddRange(entityList);
	}

	public List<T> Items { get; set; } = [];
	public int TotalRowCount { get; set; }
}
