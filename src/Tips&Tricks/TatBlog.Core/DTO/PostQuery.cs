using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;

namespace TatBlog.Core.DTO
{
    public class PostQuery
    {
        public int? AuthorId { get; set; }
        public string AuthorSlug { get; set; }

        public int PostId { get; set; }

        public int? CategoryId { get; set; }

        public string CategorySlug { get; set; }

        public int? PostedYear { get; set; }

        public int? PostedMonth { get; set; }

        public int PostedDay { get; set; }

        public int TagId { get; set; }

        public string TagSlug { get; set; }

        public bool PublishedOnly { get; set; }
        public string Keyword { get; set; }
        public string PostSlug { get; set; }
    }
}