using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Core.DTO;
using TatBlog.Core.Contracts;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class BlogRepository : IBlogRepository
{
    private readonly BlogDbContext _context;

    public BlogRepository(BlogDbContext context)
    {
        _context = context;
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
            .Include(x => x.Author);

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
    #endregion

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
    public async Task AddOrUpdateCategoryAsync(
         Category category,
            CancellationToken cancellationToken = default)
    {
        if (IsCategorySlugExistedAsync(category.Id, category.UrlSlug).Result)
            Console.WriteLine("Error: Exsited Slug");
        else

            if (category.Id > 0) // true: update || false: add
            await _context.Set<Category>()
            .Where(c => c.Id == category.Id)
            .ExecuteUpdateAsync(c => c
                .SetProperty(x => x.Name, x => category.Name)
                .SetProperty(x => x.UrlSlug, x => category.UrlSlug)
                .SetProperty(x => x.Description, x => category.Description)
                .SetProperty(x => x.ShowOnMenu, category.ShowOnMenu)
                .SetProperty(x => x.Posts, category.Posts),
                cancellationToken);
        else
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
    }

    // 1.h: Xóa một chuyên mục theo mã số cho trước
    public async Task DeleteCategoryByIdAsync(
        int CategoryId,
        CancellationToken cancellationToken = default)
    {
        await _context.Database
                .ExecuteSqlRawAsync("DELETE FROM Posts WHERE Id = " + CategoryId,
                cancellationToken);

        await _context.Set<Category>()
            .Where(t => t.Id == CategoryId)
            .ExecuteDeleteAsync(cancellationToken);
    }

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
        IPagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var categoriesQuery = _context.Set<Category>()
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published)
            });
        return await categoriesQuery.ToPagedListAsync(pagingParams, cancellationToken);
    }


    // 1.k: Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào.
    // Kết quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số bài viết
    public async Task<IList<PostItem>> CountPostsMonthAsync(
        int n,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
    // 1.m: Thêm hay cập nhật một bài viết
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

    public Task<IList<Post>> FindAllPostsWithPostQueryAsync(PostQuery pq, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    // 1.r: Đếm số lượng bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối tượng PostQuery

    public Task<int> CountPostsWithPostQueryAsync(PostQuery pq, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    // 1.s: Tìm và phân trang các bài viết thỏa mãn điều kiện tìm kiếm được cho trong
    // đối tượng PostQuery(kết quả trả về kiểu IPagedList<Post>)

    // 1.t: Tương tự câu trên nhưng yêu cầu trả về kiểu IPagedList<T>. Trong đó T
    // là kiểu dữ liệu của đối tượng mới được tạo từ đối tượng Post.Hàm này có
    // thêm một đầu vào là Func<IQueryable<Post>, IQueryable<T>> mapper
    // để ánh xạ các đối tượng Post thành các đối tượng T theo yêu cầu

    #endregion

    #region Phần C.2

    #endregion
}
