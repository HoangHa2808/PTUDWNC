using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.Contracts;

public interface IPagingParams
{
	public int PageSize { get; set; }
	public int PageNumber { get; set; }
	public string SortColumn { get; set; }
	public string SortOrder { get; set; }
}
