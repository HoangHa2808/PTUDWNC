using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class FeaturedPosts : ViewComponent
{
    private readonly IBlogRepository _blogRepository;

    public FeaturedPosts(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    // Hiển thị TOP 3 bài viết được xem nhiều nhất
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var featuredPosts = await _blogRepository.GetPopularArticlesAsync(3);
        return View(featuredPosts);
    }
}
