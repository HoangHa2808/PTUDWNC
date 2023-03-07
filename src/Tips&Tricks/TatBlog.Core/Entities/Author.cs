using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Entities;

// Thông tin tác giả của một bài viết 
public class Author : IEntity
{
public int Id { get; set; }
	public string FullName { get; set; }
	public string UrlSlug { get; set; }
	public string ImageUrl { get; set; }
	public DateTime JoinedDate { get; set; }
	public string Email { get; set; }
	public string Notes { get; set; }
	public IList<Post> Posts { get; set; }
}
