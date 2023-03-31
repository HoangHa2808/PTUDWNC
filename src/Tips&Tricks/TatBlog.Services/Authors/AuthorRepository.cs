using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Authors
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BlogDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public AuthorRepository(BlogDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // Đếm số lượng tác giả
        public async Task<int> CountAuthorAsync(
       CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .CountAsync(cancellationToken);
        }

        #region Phần C.2

        //Câu 2. B : Tìm một tác giả theo mã số
        public async Task<Author> GetAuthorByIdAsync(int Id)
        {
            return await _context.Set<Author>().FindAsync(Id);
        }

        public async Task<IList<AuthorItem>> GetAuthorsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .OrderBy(a => a.FullName)
                .Select(a => new AuthorItem()
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.Email,
                    JoinedDate = a.JoinedDate,
                    ImageUrl = a.ImageUrl,
                    UrlSlug = a.UrlSlug,
                    PostCount = a.Posts.Count(P => P.Published)
                })
                .ToListAsync(cancellationToken);
        }

        //Câu 2. C : Tìm một tác giả theo tên định danh (slug)
        public async Task<Author> GetAuthorBySlugAsync(string Slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .Where(t => t.UrlSlug == Slug)
                .FirstOrDefaultAsync(cancellationToken);
            //return await _context.Set<Author>()
            //.FirstOrDefaultAsync(t => t.UrlSlug == Slug,cancellationToken);
        }

        public async Task<Author> GetCachedAuthorByIdAsync(int authorId)
        {
            return await _memoryCache.GetOrCreateAsync(
                $"author.by-id.{authorId}",
                async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    return await GetAuthorByIdAsync(authorId);
                });
        }

        public async Task<Author> GetCachedAuthorBySlugAsync(
            string slug, CancellationToken cancellationToken = default)
        {
            return await _memoryCache.GetOrCreateAsync(
                $"author.by-slug.{slug}",
                async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    return await GetAuthorBySlugAsync(slug, cancellationToken);
                });
        }

        public async Task<IPagedList<AuthorItem>> GetPagedAuthorsAsync(
            IPagingParams pagingParams,
            string name = null,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .AsNoTracking()
                .WhereIf(!string.IsNullOrWhiteSpace(name),
                    x => x.FullName.Contains(name))
                .Select(a => new AuthorItem()
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.Email,
                    JoinedDate = a.JoinedDate,
                    ImageUrl = a.ImageUrl,
                    UrlSlug = a.UrlSlug,
                    PostCount = a.Posts.Count(p => p.Published)
                })
                .ToPagedListAsync(pagingParams, cancellationToken);
        }

        //Câu 2. D : Lấy và phân trang danh sách tác giả kèm theo số lượng bài viết của tác giả
        //đó.Kết quả trả về kiểu IPagedList<AuthorItem>.
        public async Task<IPagedList<AuthorItem>> GetPagedAuthorAsync(
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var authorQuery = _context.Set<Author>()
                .Select(x => new AuthorItem()
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    UrlSlug = x.UrlSlug,
                    ImageUrl = x.ImageUrl,
                    JoinedDate = x.JoinedDate,
                    Email = x.Email,
                    Notes = x.Notes
                });
            return await authorQuery
                .ToPagedListAsync(pageNumber, pageSize,
            nameof(Author.FullName), "DESC", cancellationToken);
        }

        public async Task<IPagedList<T>> GetPagedAuthorsAsync<T>(
        Func<IQueryable<Author>, IQueryable<T>> mapper,
        IPagingParams pagingParams,
        string name = null,
        CancellationToken cancellationToken = default)
        {
            var authorQuery = _context.Set<Author>().AsNoTracking();

            if (!string.IsNullOrEmpty(name))
            {
                authorQuery = authorQuery.Where(x => x.FullName.Contains(name));
            }

            return await mapper(authorQuery)
                .ToPagedListAsync(pagingParams, cancellationToken);
        }

        // Xoá 1 tác giả theo mã số
        public async Task<bool> DeleteAuthorByIdAsync(
         int authorId,
         CancellationToken cancellationToken = default)
        {
            return await _context.Authors
           .Where(x => x.Id == authorId)
           .ExecuteDeleteAsync(cancellationToken) > 0;
        }

        public async Task<Author> CreateOrUpdateAuthorAsync(
        Author author, CancellationToken cancellationToken = default)
        {
            if (author.Id > 0)
            {
                _context.Set<Author>().Update(author);
            }
            else
            {
                _context.Set<Author>().Add(author);
            }
            await _context.SaveChangesAsync(cancellationToken);

            return author;
        }

        //Câu 2. E : Thêm hoặc cập nhật thông tin một tác giả
        public async Task<bool> AddOrUpdateAuthorAsync(
             Author author,
             CancellationToken cancellationToken = default)
        {
            if (author.Id > 0)
            {
                _context.Authors.Update(author);
                _memoryCache.Remove($"author.by-id.{author.Id}");
            }
            else
            {
                _context.Authors.Add(author);
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> IsAuthorSlugExistedAsync(
            int id,
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .AnyAsync(x => x.Id != id && x.UrlSlug == slug,
                cancellationToken);
        }

        public async Task<bool> SetImageUrlAsync(
       int authorId, string imageUrl,
       CancellationToken cancellationToken = default)
        {
            return await _context.Authors
                .Where(x => x.Id == authorId)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(a => a.ImageUrl, a => imageUrl),
                    cancellationToken) > 0;
        }

        //Câu 2. F : Tìm danh sách N tác giả có nhiều bài viết nhất. N là tham số đầu vào.
        public async Task<IList<AuthorItem>> ListAuthorAsync(
            int N, CancellationToken cancellationToken = default)
        {
            return await _context.Authors
                 .Include(a => a.Posts)
                 .Select(x => new AuthorItem()
                 {
                     Id = x.Id,
                     FullName = x.FullName,
                     UrlSlug = x.UrlSlug,
                     ImageUrl = x.ImageUrl,
                     JoinedDate = x.JoinedDate,
                     Email = x.Email,
                     Notes = x.Notes,
                     PostCount = x.Posts.Count(p => p.Published)
                 })
                 .OrderByDescending(x => x.PostCount)
                 .Take(N)
                 .ToListAsync(cancellationToken);
        }

        #endregion Phần C.2
    }
}