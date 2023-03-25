using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Authors;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
	public class DashboardController : Controller
	{
		private readonly ILogger<PostsController> _logger;
		private readonly IBlogRepository _blogRepository;
		private readonly IMediaManager _mediaManager;
		private readonly IAuthorRepository _authorRepository;
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

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SumPost()
		{
			PostQuery pq = new PostQuery()
			{
				PostId = 5
			};
			await _blogRepository.CountPostsWithPostQueryAsync(pq);
			return RedirectToAction(nameof(Index));
		}
		//GetCategoriesAsync

	}
}
