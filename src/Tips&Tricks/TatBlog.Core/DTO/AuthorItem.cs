using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.DTO;

public class AuthorItem
{
    public int Id { get; set; }

    public string FullName { get; set; }

    public string UrlSlug { get; set; }

    public string ImageUrl { get; set; }

    public DateTime JoinedDate { get; set; }

    public string Email { get; set; }

    public string Notes { get; set; }

    public int PostCount { get; set; }
}
