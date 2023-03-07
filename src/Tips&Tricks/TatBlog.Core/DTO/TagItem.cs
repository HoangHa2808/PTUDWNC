using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.DTO;

public class TagItem
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string UrlSlug { get; set; }
	public string Description { get; set; }
	public int PostCount { get; set; }
}
