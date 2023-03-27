using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.Services.Subscribers;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly IMediaManager _mediaManager;
        private readonly IAuthorRepository _authorRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IMapper _mapper;

        public DashboardController(
            ILogger<PostsController> logger,
            IBlogRepository blogRepository,
            IMediaManager mediaManager,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            _logger = logger;
            _authorRepository = authorRepository;
            _mediaManager = mediaManager;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        //      Tổng số bài viết, số bài viết chưa xuất bản,
        //số lượng chủ đề, số lượng tác giả, số lượng bình luận đang chờ phê duyệt,
        //số lượng người theo dõi, số lượng người mới theo dõi đăng ký(lấy số liệu trong ngày)

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            ViewBag.CountPost = await _blogRepository.CountPostAsync();
            ViewBag.CountUnPublicPost = await _blogRepository.CountUnPublicPostAsync();
            ViewBag.CountCategory = await _blogRepository.CountCategoryAsync();
            ViewBag.CountAuthor = await _authorRepository.CountAuthorAsync();
            ViewBag.CountComment = await _blogRepository.CountCommentAsync();
            ViewBag.CountSubscriber = await _subscriberRepository.CountSubAsync();
            ViewBag.CountSubscriberState = await _subscriberRepository.CountSubscriberStateAsync();
            return View();
        }

        //GetCategoriesAsync
    }
}