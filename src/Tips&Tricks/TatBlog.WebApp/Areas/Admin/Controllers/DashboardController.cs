using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Authors;
using TatBlog.Services.Subscribers;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IMediaManager _mediaManager;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public DashboardController(
            IBlogRepository blogRepository,
            ISubscriberRepository subscriberRepository,
            IMediaManager mediaManager,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            _authorRepository = authorRepository;
            _subscriberRepository = subscriberRepository;
            _mediaManager = mediaManager;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

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
    }
}