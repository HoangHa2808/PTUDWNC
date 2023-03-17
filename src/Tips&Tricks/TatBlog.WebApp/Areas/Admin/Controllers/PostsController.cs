using TatBlog.Services.Blogs;
using TatBlog.Services.Authors;
using TatBlog.WebApp.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MapsterMapper;
using TatBlog.Core.Entities;
using TatBlog.Services.Media;
using FluentValidation;
using FluentValidation.AspNetCore;
using TatBlog.Data.Contexts;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class PostsController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly IMediaManager _mediaManager;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<PostEditModel> _postValidator;

        public PostsController(
            IValidator<PostEditModel> postValidator,
            ILogger<PostsController> logger,
            IBlogRepository blogRepository,
            IMediaManager mediaManager,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            _postValidator = postValidator;
            _logger = logger;
            _authorRepository = authorRepository;
            _mediaManager = mediaManager;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(
            PostFilterModel model,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
            _logger.LogInformation("Tạo điều kiện truy vấn");


            var postQuery = _mapper.Map<PostQuery>(model);
            _logger.LogInformation("Lấy danh sách bài viết từ CSDL");

            ViewBag.PostsList = await _blogRepository
                .GetPagedPostAsync(postQuery, pageNumber, pageSize);

            _logger.LogInformation("Chuẩn bị dữ liệu cho ViewModel");

            await PopulatePostFilterModelAsync(model);

            return View(model);
        }

        public async Task<IActionResult> Delete(int id = 0)
        { 
         await _blogRepository.DeletePostsByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Toggle(int id = 0)
        {
            await _blogRepository.TogglePuslishedFlagAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var post = id > 0
                ? await _blogRepository.GetPostByIdAsync(id, true)
                : null;

            var model = post == null
                ? new PostEditModel()
                : _mapper.Map<PostEditModel>(post);

            await PopulatePostEditModelAsync(model);
            return View(model);
        }

        private async Task PopulatePostEditModelAsync(PostEditModel model)
        {
            var authors = await _authorRepository.GetAuthorsAsync();
            var categories = await _blogRepository.GetCategoriesAsync();

            model.AuthorList = authors.Select(a => new SelectListItem()
            {
                Text = a.FullName,
                Value = a.Id.ToString()
            });

            model.CategoryList = categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }


        [HttpPost]
        public async Task<IActionResult> Edit(
            PostEditModel model)
        {
            var validationResult = await _postValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);

            }
            if (!ModelState.IsValid)
            {
                await PopulatePostEditModelAsync(model);
                return View(model);
            }

            var post = model.Id > 0
                ? await _blogRepository.FindPostByIDAsync(model.Id)
                : null;

            if (post == null)
            {
                post = _mapper.Map<Post>(model);

                post.Id = 0;
                post.PostedDate = DateTime.Now;
            }
            else
            {
                _mapper.Map(model, post);

                post.Category = null;
                post.ModifiedDate = DateTime.Now;
            }

            //Nếu người dùng có upload hình ảnh minh hoạ cho bài viết
            if (model.ImageFile?.Length > 0)
            {
                // Thì thực hiện việc lưu tập tin vào thư mục uploads
                var newImagePath = await _mediaManager.SaveFileAsync(
                    model.ImageFile.OpenReadStream(),
                    model.ImageFile.FileName,
                    model.ImageFile.ContentType);

                // Nếu lưu thành công, xoá tập tin hình ảnh cũ(nếu có)
                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    await _mediaManager.DeleteFileAsync(post.ImageUrl);
                    post.ImageUrl = newImagePath;
                }
            }

            await _blogRepository.CreateOrUpdatePostAsync(
                post, model.GetSelectedTags());

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPostSlug(
            int id,
            string slug)
        {
            var slugExisted = await _blogRepository
                .IsPostSlugExistedAsync(id, slug);

            return slugExisted
                ? Json($"Slug '{slug}' đã được sử dụng")
                : Json(true);
        }

        private async Task PopulatePostFilterModelAsync(PostFilterModel model)
        {
            var authors = await _authorRepository.GetAuthorsAsync();
            var categories = await _blogRepository.GetCategoriesAsync();

            model.AuthorList = authors.Select(a => new SelectListItem()
            {
                Text = a.FullName,
                Value = a.Id.ToString()
            });

            model.CategoryList = categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }

    }
}
