using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Data.Contexts;

namespace TatBlog.Services.Subscriber
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly BlogDbContext _context;

        public SubscriberRepository(BlogDbContext context)
        {
            _context = context;
        }
    }
}
