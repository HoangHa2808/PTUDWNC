using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Entities;
using TatBlog.Core.DTO;
using TatBlog.Core.Contracts;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace TatBlog.Services.Blogs;

public class BlogRepository : IBlogRepository
{
    private readonly BlogDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public BlogRepository(BlogDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    #region Phần B.Hướng dẫn

    // Tìm bài viết có tên định danh là 'slug'
    // và được đăng vào tháng 'month' năm 'year'
    public async Task<Post> GetPostAsync(
        int year, int month,
        string slug, CancellationToken cancellationToken = default)
    {
        IQueryable<Post> postsQuery = _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tags);

        if (year > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
        }

        if (month > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
        }

        return await postsQuery.FirstOrDefaultAsync(cancellationToken);
    }

    // Tìm top N bài viết phổ biến được nhiều người xem nhất
    public async Task<IList<Post>> GetPopularArticlesAsync(
        int numPosts, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(x => x.Author)
            .Include(x => x.Category)
            .OrderByDescending(p => p.ViewCount)
            .Take(numPosts)
            .ToListAsync(cancellationToken);
    }

    // Kiểm tra xem tên định danh của bài viết đã có hay chưa
    public async Task<bool> IsPostSlugExistedAsync(
        int postId, string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .AnyAsync(x => x.Id != postId && x.UrlSlug == slug,
            cancellationToken);
    }

    // Tăng số lượt xem của một bài viết
    public async Task IncreaseViewCountAsync(
        int postId, CancellationToken cancellationToken = default)
    {
        await _context.Set<Post>()
            .Where(x => x.Id == postId)
            .ExecuteUpdateAsync(p => p.SetProperty(
                x => x.ViewCount, x => x.ViewCount + 1),
                cancellationToken);
    }

    // Lấy danh sách chuyên mục và số lượng bài viết
    // thuộc từng chuyên mục/chủ đề
    public async Task<IList<CategoryItem>> GetCategoriesAsync(bool showOnMenu = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Category> categories = _context.Set<Category>();

        if (showOnMenu)
        {
            categories = categories.Where(x => x.ShowOnMenu);
        }

        return await categories
            .OrderBy(x => x.Name)
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                ShowOnMenu = x.ShowOnMenu,
                PostCount = x.Posts.Count(p => p.Published)
            })
            .ToListAsync(cancellationToken);
    }

    // Lấy danh sách từ khóa/thẻ và phân trang theo
    // các tham số pagingParams
    public async Task<IPagedList<TagItem>> GetPagedTagsAsync(
        IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        var tagQuery = _context.Set<Tag>()
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published)
            });

        return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
    }

    #endregion Phần B.Hướng dẫn

    //public Task<IList<TagItem>> GetTagsItemsAsync(string slug, CancellationToken cancellationToken = default)
    //{
    //	IQueryable<Tag> tagItemsQuery = _context.Set<Tag>()
    //		.Include(x => x.Id)
    //		.Include(x => x.Name);
    //	return tagItemsQuery;
    //	.OrderBy(x => x.Name)
    //	.Select(x => new TagItem()
    //	{
    //		Id = x.Id,
    //		Name = x.Name,
    //		UrlSlug = x.UrlSlug,
    //		Description = x.Description,
    //		PostCount = x.Posts.Count(p => p.Published)
    //	})
    //.ToListAsync(cancellationToken);
    //}

    #region Phần C.1.Thực hành

    #region Tag

    // Cau 1.a: Tìm một thẻ(Tag) theo tên định danh(slug)
    public async Task<Tag> FindTagWithSlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
    {
        IQueryable<Tag> tagsQuery = _context.Set<Tag>();
        if (!string.IsNullOrWhiteSpace(slug))
        {
            tagsQuery = tagsQuery.Include(x => x.Posts).Where(x => x.UrlSlug == slug);
        }
        return await tagsQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Tag> GetTagByIdAsync(int Id)
    {
        return await _context.Set<Tag>().FindAsync(Id);
    }

    // Cau 1.c: Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó
    public async Task<IList<TagItem>> GetTagsItemsAsync(
        CancellationToken cancellationToken = default)
    {
        var tagItemsQuery = _context.Set<Tag>();

        return await tagItemsQuery
            .OrderBy(x => x.Id)
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published)
            })
            .ToListAsync(cancellationToken);
    }

    // Cau 1.d: Xóa một thẻ theo mã cho trước
    public async Task DeleteTagWithIdAsync(
     int id,
     CancellationToken cancellationToken = default)
    {
        await _context.Database
                .ExecuteSqlRawAsync("DELETE FROM PostTags WHERE TagId = " + id, cancellationToken);

        await _context.Set<Tag>()
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<bool> DeleteTagByIdAsync(
     int tagId,
     CancellationToken cancellationToken = default)
    {
        var tag = await _context.Set<Tag>().FindAsync(tagId);
        if (tag is null) return false;
        _context.Set<Tag>().Remove(tag);
        var rowsCount = await _context.SaveChangesAsync(cancellationToken);
        return rowsCount > 0;
    }

    public async Task<Tag> CreateOrUpdateTagAsync(
        Tag tag, CancellationToken cancellationToken = default)
    {
        if (tag.Id > 0)
        {
            _context.Set<Tag>().Update(tag);
        }
        else
        {
            _context.Set<Tag>().Add(tag);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return tag;
    }

    public async Task<bool> IsTagSlugExistedAsync(
   int tagId,
   string slug,
   CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
            .AnyAsync(x => x.Id != tagId && x.UrlSlug == slug, cancellationToken);
    }

    public async Task<Tag> GetTagAsync(
       string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
            .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
    }

    public async Task<IPagedList<TagItem>> GetPagedTagAsync(
           int pageNumber = 1,
           int pageSize = 10,
           CancellationToken cancellationToken = default)
    {
        var tagQuery = _context.Set<Tag>()
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count
            });
        return await tagQuery
            .ToPagedListAsync(pageNumber, pageSize,
        nameof(Tag.Name), "DESC", cancellationToken);
    }

    public async Task<IPagedList<TagItem>> GetPagedTagsAsync(
          IPagingParams pagingParams,
          string name = null,
          CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                x => x.Name.Contains(name))
            .Select(a => new TagItem()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                UrlSlug = a.UrlSlug,
                PostCount = a.Posts.Count(p => p.Published)
            })
            .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<Tag> GetCachedTagByIdAsync(int tagId)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"tag.by-id.{tagId}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetTagByIdAsync(tagId);
            });
    }

    public async Task<Tag> GetCachedTagBySlugAsync(
        string slug, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"tag.by-slug.{slug}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await FindTagWithSlugAsync(slug, cancellationToken);
            });
    }

    public async Task<bool> AddOrUpdateTagAsync(
            Tag tag,
            CancellationToken cancellationToken = default)
    {
        if (tag.Id > 0)
        {
            _context.Tags.Update(tag);
            _memoryCache.Remove($"tag.by-id.{tag.Id}");
        }
        else
        {
            _context.Tags.Add(tag);
        }

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    #endregion Tag

    #region Category

    // Câu 1.e: Tìm một chuyên mục (Category) theo tên định danh(slug)
    public async Task<Category> FindCategoryByUrlAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Category> categoriesQuery = _context.Set<Category>();
        {
            if (!string.IsNullOrWhiteSpace(slug))
            {
                categoriesQuery = categoriesQuery.Where(x => x.UrlSlug == slug);
            }
        }
        return await categoriesQuery.FirstOrDefaultAsync(cancellationToken);
    }

    //public async Task<Category> GetCategoryByIdAsync(int categoryId)
    //{
    //    return await _context.Set<Category>().FindAsync(categoryId);
    //}
    // 1.f: Tìm 1 chuyên mục theo mã số

    public async Task<Category> FindCategoryByIDAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    // 1.g: Thêm hoặc cập nhật một chuyên mục
    public async Task<Category> CreateOrUpdateCategoryAsync(
        Category category, CancellationToken cancellationToken = default)
    {
        if (category.Id > 0)
        {
            _context.Set<Category>().Update(category);
        }
        else
        {
            _context.Set<Category>().Add(category);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return category;
    }

    //public async Task AddOrUpdateCategoryAsync(
    //     Category category,
    //        CancellationToken cancellationToken = default)
    //{
    //    if (IsCategorySlugExistedAsync(category.Id, category.UrlSlug).Result)
    //        Console.WriteLine("Error: Exsited Slug");
    //    else

    //        if (category.Id > 0) // true: update || false: add
    //        await _context.Set<Category>()
    //        .Where(c => c.Id == category.Id)
    //        .ExecuteUpdateAsync(c => c
    //            .SetProperty(x => x.Name, x => category.Name)
    //            .SetProperty(x => x.UrlSlug, x => category.UrlSlug)
    //            .SetProperty(x => x.Description, x => category.Description)
    //            .SetProperty(x => x.ShowOnMenu, category.ShowOnMenu)
    //            .SetProperty(x => x.Posts, category.Posts),
    //            cancellationToken);
    //    else
    //    {
    //        _context.Categories.Add(category);
    //        _context.SaveChanges();
    //    }
    //}

    // 1.h: Xóa một chuyên mục theo mã số cho trước

    public async Task<bool> DeleteCategoryByIdAsync(
     int categoryId,
     CancellationToken cancellationToken = default)
    {
        var category = await _context.Set<Category>().FindAsync(categoryId);
        if (category is null) return false;
        _context.Set<Category>().Remove(category);
        var rowsCount = await _context.SaveChangesAsync(cancellationToken);
        return rowsCount > 0;
    }

    public async Task<bool> ToggleShowOnMenuFlagAsync(
      int categoryId,
      CancellationToken cancellationToken = default)
    {
        var category = await _context.Set<Category>().FindAsync(categoryId);
        if (category is null) return false;
        category.ShowOnMenu = !category.ShowOnMenu;
        await _context.SaveChangesAsync(cancellationToken);
        return category.ShowOnMenu;
    }

    //public async Task DeleteCategoryByIdAsync(
    //    int CategoryId,
    //    CancellationToken cancellationToken = default)
    //{
    //    await _context.Database
    //            .ExecuteSqlRawAsync("DELETE FROM Posts WHERE Id = " + CategoryId,
    //            cancellationToken);

    //    await _context.Set<Category>()
    //        .Where(t => t.Id == CategoryId)
    //        .ExecuteDeleteAsync(cancellationToken);
    //}

    // 1.i: Kiểm tra tên định danh (slug)
    //của một chuyên mục đã tồn tài hay chưa

    public async Task<bool> IsCategorySlugExistedAsync(
    int categoryId,
    string slug,
    CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AnyAsync(x => x.Id != categoryId && x.UrlSlug == slug, cancellationToken);
    }

    // 1.j: Lấy và phân trang danh sách chuyên mục
    //kết quả trả về kiểu IPagedList<CategoryItem>
    public async Task<IPagedList<CategoryItem>> GetPagedCategoryAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var categoriesQuery = _context.Set<Category>()
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                ShowOnMenu = x.ShowOnMenu,
                PostCount = x.Posts.Count(p => p.Published)
            });
        return await categoriesQuery.ToPagedListAsync(
            pageNumber, pageSize,
            nameof(Category.Name), "DESC",
            cancellationToken);
    }

    public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
          IPagingParams pagingParams,
          string name = null,
          CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                x => x.Name.Contains(name))
            .Select(a => new CategoryItem()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                UrlSlug = a.UrlSlug,
                ShowOnMenu = a.ShowOnMenu,
                PostCount = a.Posts.Count(p => p.Published)
            })
            .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<Category> GetCachedCategoryByIdAsync(int categoryId)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"category.by-id.{categoryId}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await FindCategoryByIDAsync(categoryId);
            });
    }

    public async Task<Category> GetCachedCategoryBySlugAsync(
        string slug, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"category.by-slug.{slug}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await FindCategoryByUrlAsync(slug, cancellationToken);
            });
    }

    public async Task<bool> AddOrUpdateCategoryAsync(
            Category category,
            CancellationToken cancellationToken = default)
    {
        if (category.Id > 0)
        {
            _context.Categories.Update(category);
            _memoryCache.Remove($"category.by-id.{category.Id}");
        }
        else
        {
            _context.Categories.Add(category);
        }

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    #endregion Category

    #region Post

    public async Task<bool> DeletePostsByIdAsync(
      int postId,
      CancellationToken cancellationToken = default)
    {
        var post = await _context.Set<Post>().FindAsync(postId);
        if (post is null) return false;
        _context.Set<Post>().Remove(post);
        var rowsCount = await _context.SaveChangesAsync(cancellationToken);
        return rowsCount > 0;
    }

    public async Task<bool> TogglePuslishedFlagAsync(
      int postId,
      CancellationToken cancellationToken = default)
    {
        var post = await _context.Set<Post>().FindAsync(postId);
        if (post is null) return false;
        post.Published = !post.Published;
        await _context.SaveChangesAsync(cancellationToken);
        return post.Published;
    }

    public async Task<bool> SetImageUrlAsync(
       int postId, string imageUrl,
       CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .Where(x => x.Id == postId)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(a => a.ImageUrl, a => imageUrl),
                cancellationToken) > 0;
    }

    // 1.k: Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào.
    // Kết quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số bài viết
    public async Task<IList<PostItem>> CountPostsMonthAsync(
    int n,
    CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .GroupBy(x => new { x.PostedDate.Year, x.PostedDate.Month })
            .Select(g => new PostItem()
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                PostCount = g.Count(x => x.Published)
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<Post> GetPostByIdAsync(
        int postId, bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        if (!includeDetails)
        {
            return await _context.Set<Post>().FindAsync(postId);
        }

        return await _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
    }

    public async Task<Post> GetPostBySlugAsync(
        string postSlug, bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        if (includeDetails)
            return await _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Tags)
                .Where(x => x.UrlSlug == postSlug)
                .FirstOrDefaultAsync(cancellationToken);
        else
            return await _context.Set<Post>()
                .Where(x => x.UrlSlug == postSlug)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // 1.l: Tìm một bài viết theo mã số
    public async Task<Post> FindPostByIDAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    //1.m: Thêm hay cập nhật một bài viết
    public async Task AddOrUpdatePostAsync(
       Post postName,
       CancellationToken cancellationToken = default)
    {
        if (IsPostSlugExistedAsync(postName.Id, postName.UrlSlug).Result)
            Console.WriteLine("Error: Existed Slug");
        else

           if (postName.Id > 0) // true: update || false: add
            await _context.Set<Post>()
            .Where(p => p.Id == postName.Id)
            .ExecuteUpdateAsync(p => p
                .SetProperty(x => x.Title, x => postName.Title)
                .SetProperty(x => x.UrlSlug, x => postName.UrlSlug)
                .SetProperty(x => x.ShortDescription, x => postName.ShortDescription)
                .SetProperty(x => x.Description, x => postName.Description)
                .SetProperty(x => x.Meta, x => postName.Meta)
                .SetProperty(x => x.ImageUrl, x => postName.ImageUrl)
                .SetProperty(x => x.ViewCount, x => postName.ViewCount)
                .SetProperty(x => x.Published, x => postName.Published)
                .SetProperty(x => x.PostedDate, x => postName.PostedDate)
                .SetProperty(x => x.ModifiedDate, x => postName.ModifiedDate)
                .SetProperty(x => x.CategoryId, x => postName.CategoryId)
                .SetProperty(x => x.AuthorId, x => postName.AuthorId)
                .SetProperty(x => x.Category, x => postName.Category)
                .SetProperty(x => x.Author, x => postName.Author)
                .SetProperty(x => x.Tags, x => postName.Tags),
                cancellationToken);
        else
        {
            _context.AddRange(postName);
            _context.SaveChanges();
        }
    }

    public async Task<bool> AddOrUpdatePostsAsync(
           Post post,
           CancellationToken cancellationToken = default)
    {
        if (post.Id > 0)
        {
            _context.Posts.Update(post);
            _memoryCache.Remove($"post.by-id.{post.Id}");
        }
        else
        {
            _context.Posts.Add(post);
        }

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Post> CreateOrUpdatePostAsync(
            Post post, IEnumerable<string> tags,
            CancellationToken cancellationToken = default)
    {
        if (post.Id > 0)
        {
            await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
        }
        else
        {
            post.Tags = new List<Tag>();
        }

        var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => new
            {
                Name = x,
                Slug = GenerateSlug(x)
            })
            .GroupBy(x => x.Slug)
            .ToDictionary(g => g.Key, g => g.First().Name);

        foreach (var kv in validTags)
        {
            if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

            var tag = await FindTagWithSlugAsync(kv.Key, cancellationToken) ?? new Tag()
            {
                Name = kv.Value,
                Description = kv.Value,
                UrlSlug = kv.Key
            };

            post.Tags.Add(tag);
        }

        post.Tags = post.Tags.Where(t => validTags.ContainsKey(t.UrlSlug)).ToList();

        if (post.Id > 0)
            _context.Update(post);
        else
            _context.Add(post);

        await _context.SaveChangesAsync(cancellationToken);

        return post;
    }

    private string GenerateSlug(string s)
    {
        return s.ToLower().Replace(".", "dot").Replace(" ", "-");
    }

    // 1.n: Chuyển đổi trạng thái Published của bài viết
    public async Task PublishedAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<Post>()
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(p => p.Published, p => !p.Published),
                cancellationToken);
    }

    // 1.o: Lấy ngẫu nhiên N bài viết. N là tham số đầu vào
    public async Task<IList<Post>> GetRandomPostsAsync(
        int n,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .OrderBy(x => Guid.NewGuid())
            .Take(n)
            .ToListAsync();
    }

    // 1.p: Tạo lớp PostQuery để lưu trữ các điều kiện tìm kiếm bài viết. Chẳng hạn:mã tác giả,
    // mã chuyên mục, tên ký hiệu chuyên mục, năm/tháng đăng bài, từ khóa, …
    public async Task<IList<Post>> GetAllPostsByPostQuery(
        PostQuery pquery, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(c => c.Category)
            .Include(c => c.Tags)
            .WhereIf(pquery.AuthorId > 0, p => p.AuthorId == pquery.AuthorId)
            .WhereIf(pquery.PostId > 0, p => p.Id == pquery.PostId)
            .WhereIf(pquery.CategoryId > 0, p => p.CategoryId == pquery.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(pquery.CategorySlug), p => p.Category.UrlSlug ==
            pquery.CategorySlug)
            .WhereIf(pquery.PostedYear > 0, p => p.PostedDate.Year == pquery.PostedYear)
            .WhereIf(pquery.PostedMonth > 0, p => p.PostedDate.Month == pquery.PostedMonth)
            .WhereIf(pquery.TagId > 0, p => p.Tags.Any(x => x.Id == pquery.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(pquery.TagSlug), p => p.Tags.Any(x => x.UrlSlug ==
            pquery.TagSlug))
            .ToListAsync(cancellationToken);
    }

    // 1.q: Tìm tất cả bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối tượng
    // PostQuery(kết quả trả về kiểu IList<Post>)

    public async Task<IList<Post>> FindAllPostsWithPostQueryAsync(PostQuery pq, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
                .Include(c => c.Category)
                .Include(t => t.Tags)
                .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
                .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
                .WhereIf(pq.CategoryId > 0, p => p.CategoryId == pq.CategoryId)
                .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug == pq.CategorySlug)
                .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
                .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
                .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
                .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
                .ToListAsync(cancellationToken);
    }

    // 1.r: Đếm số lượng bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối tượng PostQuery

    public async Task<int> CountPostsWithPostQueryAsync(PostQuery pq,
             CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(c => c.Category)
            .Include(t => t.Tags)
            .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
            .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
            .WhereIf(pq.CategoryId > 0, p => p.CategoryId == pq.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug == pq.CategorySlug)
            .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
            .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
            .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
            .CountAsync(cancellationToken);
    }

    // 1.s: Tìm và phân trang các bài viết thỏa mãn điều kiện tìm kiếm được cho trong
    // đối tượng PostQuery(kết quả trả về kiểu IPagedList<Post>)
    public async Task<IPagedList<Post>> GetPagedsPostAsync(
        PostQuery pq, IPagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        return await FilterPost(pq)
                .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public IQueryable<Post> FilterPost(PostQuery pq)
    {
        var query = _context.Set<Post>()
            .Include(c => c.Category)
            .Include(t => t.Tags)
            .Include(a => a.Author);
        return query
            .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.AuthorSlug), p => p.Author.UrlSlug == pq.AuthorSlug)
            .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
            .WhereIf(pq.CategoryId > 0, p => p.CategoryId == pq.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug == pq.CategorySlug)
            .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
            .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
            .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
            .WhereIf(pq.PublishedOnly, p => p.Published == pq.PublishedOnly)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.PostSlug), p => p.UrlSlug == pq.PostSlug)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.Keyword), p => p.Title.Contains(pq.Keyword) ||
                    p.ShortDescription.Contains(pq.Keyword) ||
                    p.Description.Contains(pq.Keyword) ||
                    p.Category.Name.Contains(pq.Keyword) ||
                    p.Tags.Any(t => t.Name.Contains(pq.Keyword)));
    }

    public async Task<IPagedList<T>> GetPagedPostsAsync<T>(
   PostQuery condition,
   IPagingParams pagingParams,
   Func<IQueryable<Post>, IQueryable<T>> mapper)
    {
        var posts = FilterPost(condition);
        var projectedPosts = mapper(posts);

        return await projectedPosts.ToPagedListAsync(pagingParams);
    }

    public async Task<IPagedList<Post>> GetPagedPostAsync(
            PostQuery pq,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
    {
        return await FilterPost(pq)
            .ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Post.PostedDate), "DESC",
                cancellationToken);
    }

    //public  Task<IPagedList<Post>> GetPagedPostQueryAsync(
    //        PostQuery pq,
    //        IPagingParams pagingParams,
    //        CancellationToken cancellationToken = default)
    //{
    //    var query = _context.Set<Post>()
    //        .Include(c => c.Category)
    //        .Include(t => t.Tags)
    //        .Include(a => a.Author);
    //    return query
    //        .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
    //        .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
    //        .WhereIf(pq.CategoryId > 0, p => p.CategoryId == pq.CategoryId)
    //        .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug == pq.CategorySlug)
    //        .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
    //        .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
    //        .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
    //        .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
    //        .ToPagedListAsync(pagingParams, cancellationToken);
    //}

    //public async Task<IPagedList<Post>> GetPagedPostsAsync(
    //        PostQuery pq,
    //        int pageNumber = 1,
    //        int pageSize = 10,
    //        CancellationToken cancellationToken = default)
    //{
    //    return await FilterPost(pq)
    //        .ToPagedListAsync(
    //            pageNumber, pageSize,
    //            nameof(Post.PostedDate), "DESC",
    //            cancellationToken);
    //}

    //public async Task<IPagedList<Post>> GetPagedPostQueryAsync(
    //        PostQuery pq,
    //        int pageNumber,
    //        int pageSize,
    //        CancellationToken cancellationToken = default)
    //{
    //    return await _context.Set<Post>()
    //        .Include(c => c.Category)
    //        .Include(t => t.Tags)
    //        .Include(a => a.Author)
    //        .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
    //        .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
    //        .WhereIf(pq.CategoryId > 0, p => p.CategoryId == pq.CategoryId)
    //        .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug == pq.CategorySlug)
    //        .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
    //        .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
    //        .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
    //        .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
    //        .WhereIf(pq.PublishedOnly, p => p.Published == true)
    //        .ToPagedListAsync(pageNumber, pageSize,"Id","DESC",cancellationToken);
    //}

    // 1.t: Tương tự câu trên nhưng yêu cầu trả về kiểu IPagedList<T>. Trong đó T
    // là kiểu dữ liệu của đối tượng mới được tạo từ đối tượng Post.Hàm này có
    // thêm một đầu vào là Func<IQueryable<Post>, IQueryable<T>> mapper
    // để ánh xạ các đối tượng Post thành các đối tượng T theo yêu cầu

    public async Task<IPagedList<T>> GetPagedPostQueryAsync<T>(
            PostQuery pq,
            Func<IQueryable<Post>, IQueryable<T>> mapper,
            CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> GetCachedPostByIdAsync(int postId)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"post.by-id.{postId}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await FindPostByIDAsync(postId);
            });
    }

    #endregion Post

    #endregion Phần C.1.Thực hành

    #region Comment

    // Tìm 1 bình luận theo mã số
    public async Task<IList<Comment>> GetCommentsAsync(bool isApproved = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Comment> comments = _context.Set<Comment>();

        if (isApproved)
        {
            comments = comments.Where(x => x.IsApproved);
        }

        return await comments
            .OrderBy(x => x.Name)
            .Select(x => new Comment()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsApproved = x.IsApproved,
                PostId = x.PostId,
                PostedDate = x.PostedDate,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Comment> GetCommentByIDAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Comment>()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Comment> GetCachedCommentByIdAsync(int commentId)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"comment.by-id.{commentId}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetCommentByIDAsync(commentId);
            });
    }

    // Thêm hoặc cập nhật một bình luận
    public async Task<Comment> CreateOrUpdateCommentAsync(
        Comment comment, CancellationToken cancellationToken = default)
    {
        if (comment.Id > 0)
        {
            _context.Set<Comment>().Update(comment);
        }
        else
        {
            _context.Set<Comment>().Add(comment);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return comment;
    }

    public async Task<bool> AddOrUpdateCommentAsync(
            Comment comment,
            CancellationToken cancellationToken = default)
    {
        if (comment.Id > 0)
        {
            _context.Comments.Update(comment);
            _memoryCache.Remove($"author.by-id.{comment.Id}");
        }
        else
        {
            _context.Comments.Add(comment);
        }

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> ChangeCommentByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Comment>()
            .Where(comment => comment.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(comment => comment.IsApproved, comment => !comment.IsApproved), cancellationToken) > 0;
    }

    public async Task<IPagedList<Comment>> GetPagedCommentAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var commentQuery = _context.Set<Comment>();

        return await commentQuery.ToPagedListAsync(
            pageNumber, pageSize,
            nameof(Comment.Name), "DESC",
            cancellationToken);
    }

    public async Task<IPagedList<Comment>> GetPagedCommentsAsync(
            IPagingParams pagingParams,
            string name = null,
            CancellationToken cancellationToken = default)
    {
        return await _context.Set<Comment>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                x => x.Name.Contains(name))
            .Select(a => new Comment()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                PostedDate = a.PostedDate,
                PostId = a.PostId
            })
            .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<IList<Comment>> GetPostCommentsAsync(int commentId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Comment>()
            .Where(c => c.PostId == commentId && c.IsApproved)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteCommentByIdAsync(
        int commentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Comments
           .Where(x => x.Id == commentId)
           .ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public async Task<bool> ToggleApprovedFlagAsync(
      int commentId,
      CancellationToken cancellationToken = default)
    {
        var comment = await _context.Set<Comment>().FindAsync(commentId);
        if (comment is null) return false;
        comment.IsApproved = !comment.IsApproved;
        await _context.SaveChangesAsync(cancellationToken);
        return comment.IsApproved;
    }

    public async Task<bool> IsCommentExistedAsync(
   int commentId,
   string slug,
   CancellationToken cancellationToken = default)
    {
        return await _context.Set<Comment>()
            .AnyAsync(x => x.Id != commentId && x.Description == slug, cancellationToken);
    }

    #endregion Comment

    #region Dashboard

    //  Tổng số bài viết, số bài viết chưa xuất bản,
    //số lượng chủ đề, số lượng tác giả, số lượng bình luận đang chờ phê duyệt

    public async Task<int> CountPostAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountUnPublicPostAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .CountAsync(x => !x.Published, cancellationToken);
    }

    public async Task<int> CountCategoryAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountCommentAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Comment>()
            .CountAsync(x => !x.IsApproved, cancellationToken);
    }

    // Số lượng người theo dõi, số lượng người mới theo dõi đăng ký(lấy số liệu trong ngày)
    //public async Task<int> CountSubAsync(
    //   CancellationToken cancellationToken = default)
    //{
    //    return await _context.Set<Subscriber>()
    //        .CountAsync(cancellationToken);
    //}

    //public async Task<int> CountSubscriberStateAsync(
    //   CancellationToken cancellationToken = default)
    //{
    //    return await _context.Set<Subscriber>()
    //            .CountAsync(x => x.SubscribedDate.CompareTo(DateTime.Now) == 0, cancellationToken);
    //}

    #endregion Dashboard
}