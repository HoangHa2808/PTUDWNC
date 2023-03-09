using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class Archives : ViewComponent
{
    private readonly IBlogRepository _blogRepository;

    public Archives(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    //Hiển thị danh sách 12 tháng gần nhất và số lượng
    // bài viết trong mỗi tháng dưới dạng các liên kết
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var archives = await _blogRepository.CountPostsMonthAsync(12);
        return View(archives);
    }
}
