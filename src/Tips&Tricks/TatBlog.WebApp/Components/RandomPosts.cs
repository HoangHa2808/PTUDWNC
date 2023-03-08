using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class RandomPosts : ViewComponent
{
    private readonly IBlogRepository _blogRepository;

    public RandomPosts(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    // Hiển thị TOP 5 bài viết ngẫu nhiên
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var randomPosts = await _blogRepository.GetRandomPostsAsync(5);
        return View(randomPosts);
    }
}
