using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Collections;

public class PagedList<T> : IPagedList<T>
{
	private readonly List<T> _subset = new();

	public PagedList(
		IList<T> items,
		int pageNumber,
		int pageSize,
		int totalCount
	)
	{
		PageNumber = pageNumber;
		PageSize = pageSize;
		TotalItemCount = totalCount;

		_subset.AddRange(items);
	}

	public T this[int index] => _subset[index];

	public virtual int Count => _subset.Count;

	public int PageCount
	{
		get
		{
			if (PageSize == 0) return 0;
			var total = TotalItemCount / PageSize;
			if (TotalItemCount % PageSize > 0) total++;
			return total;
		}
		set { }
	}

	public int TotalItemCount { get; }

	public int PageIndex { get; set; }

	public int PageNumber
	{
		get => PageIndex + 1;
		set => PageIndex = value - 1;
	}

	public int PageSize { get; set; }

	public bool HasPreviousPage => PageIndex > 0;

	public bool HasNextPage => PageIndex < (PageCount - 1);

	public bool IsFirstPage => PageIndex <= 0;

	public bool IsLastPage => PageIndex >= (PageCount - 1);

	public int FirstItemIndex => PageIndex * PageSize + 1;

	public int LastItemIndex => Math.Min(TotalItemCount, PageIndex * PageSize + PageSize);

	public IEnumerator<T> GetEnumerator()
	{
		return _subset.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
