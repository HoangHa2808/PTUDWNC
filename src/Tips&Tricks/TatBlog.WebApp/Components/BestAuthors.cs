using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class BestAuthors : ViewComponent
{
    private readonly IBlogRepository _blogRepository;

    public BestAuthors(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    // Hiển thị TOP 4 tác giả có nhiều bài viết nhất
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var bestAuthors = await _blogRepository.();
        return View(bestAuthors);
    }
}
