using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class TagCloud : ViewComponent
{
    private readonly IBlogRepository _blogRepository;

    public TagCloud(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    // Hiển thị danh sách các thẻ (tag)
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var randomPosts = await _blogRepository.GetTagsItemsAsync();
        return View(randomPosts);
    }
}
