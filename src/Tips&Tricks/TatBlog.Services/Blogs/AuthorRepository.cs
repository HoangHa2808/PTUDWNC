using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Data.Contexts;

namespace TatBlog.Services.Blogs
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BlogDbContext _context;

        public AuthorRepository(BlogDbContext context)
        {
            _context = context;
        }
    }
}
