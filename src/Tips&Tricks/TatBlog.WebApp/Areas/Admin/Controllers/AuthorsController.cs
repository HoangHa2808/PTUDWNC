//using FluentValidation;
//using FluentValidation.AspNetCore;
//using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using TatBlog.Core.Entities;
using TatBlog.Services.Authors;
using TatBlog.Services.Blogs;
//using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class AuthorsController : Controller
    {
        //private readonly ILogger<PostsController> _logger;
        private readonly IBlogRepository _blogRepository;
        //private readonly IMediaManager _mediaManager;
        private readonly IAuthorRepository _authorRepository;
        //private readonly IMapper _mapper;

        public AuthorsController(
            //ILogger<PostsController> logger,
            IBlogRepository blogRepository,
            //IMediaManager mediaManager,
            IAuthorRepository authorRepository,
            //IMapper mapper)
        {
            _logger = logger;
            _authorRepository = authorRepository;
            _mediaManager = mediaManager;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
           
            var model = await _authorRepository.GetPagedAuthorsAsync(pageNumber, pageSize);

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var author = id > 0
                ? await _authorRepository.GetAuthorByIdAsync(id)
                : null;

            // Tạo view model từ dữ liệu của chủ đề
            var model = author == null
                ? new AuthorEditModel()
                : _mapper.Map<AuthorEditModel>(author);

            // Gán các giá trị khác cho view model
            await PopulateAuthorEditModelAsync(model);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            await _authorRepository.DeleteAuthorByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Toggle(int id = 0)
        //{
        //    await _blogRepository.TogglePuslishedFlagAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}

        private async Task PopulateAuthorEditModelAsync(AuthorEditModel model)
        {
            //var authors = await _authorRepository.GetAuthorsAsync();
            var categories = await _blogRepository.GetCategoriesAsync();

            model.CategoryList = categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }


        [HttpPost]
        public async Task<IActionResult> Edit(
            [FromQuery] IValidator<AuthorEditModel> authorValidator,
            AuthorEditModel model)
        {
            var validationResult = await authorValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);

            }
            if (!ModelState.IsValid)
            {
                await PopulateAuthorEditModelAsync(model);
                return View(model);
            }

            var author = model.Id > 0
                ? await _authorRepository.GetAuthorByIdAsync(model.Id)
                : null;

            if (author == null)
            {
                author = _mapper.Map<Author>(model);

                author.Id = 0;
                author.JoinedDate = DateTime.Now;
            }
            else
            {
                _mapper.Map(model, author);
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
                    await _mediaManager.DeleteFileAsync(author.ImageUrl);
                    author.ImageUrl = newImagePath;
                }
            }

            await _authorRepository.AddOrUpdateAuthorAsync(
                author);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPostSlug(
            int id,
            string slug)
        {
            var slugExisted = await _blogRepository
                .IsCategorySlugExistedAsync(id, slug);

            return slugExisted
                ? Json($"Slug '{slug}' đã được sử dụng")
                : Json(true);
        }

        private async Task PopulatePostFilterModelAsync(AuthorEditModel model)
        {
            var categories = await _blogRepository.GetCategoriesAsync();

            model.CategoryList = categories.Select(a => new SelectListItem()
            {
                Text = a.Name,
                Value = a.Id.ToString()
            });
        }
    }
}
