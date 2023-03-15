using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Services.Authors;

namespace TatBlog.WebApp.Components;

public class BestAuthors : ViewComponent
{
    private readonly IAuthorRepository _authorRepository;

    public BestAuthors(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    //Hiển thị TOP 4 tác giả có nhiều bài viết nhất
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var bestAuthors = await _authorRepository.ListAuthorAsync(4);
        return View(bestAuthors);
    }
}
