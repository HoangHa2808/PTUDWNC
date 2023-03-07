using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.Contracts;

public interface IPagedList
{
	public int PageCount { get; set; }
	public int TotalItemCount { get; }
	public int PageIndex { get; set; }
	public int PageNumber { get; set; }
	public int PageSize { get; set; }
	public bool HasPreviousPage { get; }
	public bool HasNextPage { get; }
	public bool IsFirstPage { get; }
	public bool IsLastPage { get; }
	public int FirstItemIndex { get; }
	public int LastItemIndex { get; }
}

public interface IPagedList<out T> : IPagedList, IEnumerable<T>
{
	public T this[int index] { get; }
	public int Count { get; }
}
