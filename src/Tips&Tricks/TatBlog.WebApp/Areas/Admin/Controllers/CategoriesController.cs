using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Authors;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IValidator<CategoryEditModel> _categoryValidator;
        private readonly IBlogRepository _blogRepository;
        private readonly IMediaManager _mediaManager;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public CategoriesController(
            ILogger<PostsController> logger,
            IValidator<CategoryEditModel> categoryValidator,
            IBlogRepository blogRepository,
            IMediaManager mediaManager,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            _logger = logger;
            _categoryValidator = categoryValidator;
            _authorRepository = authorRepository;
            _mediaManager = mediaManager;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
            var model = await _blogRepository.GetPagedCategoryAsync(pageNumber, pageSize);
     
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var category = id > 0
                ? await _blogRepository.FindCategoryByIDAsync(id)
                : null;

            // Tạo view model từ dữ liệu của chủ đề
            var model = category == null
                ? new CategoryEditModel()
                : _mapper.Map<CategoryEditModel>(category);

            // Gán các giá trị khác cho view model
            return View(model);
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            await _blogRepository.DeleteCategoryByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Toggle(int id = 0)
        {
            await _blogRepository.ToggleShowOnMenuFlagAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            CategoryEditModel model)
        {
            var validationResult = await _categoryValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);

            }
            var category = model.Id > 0
                ? await _blogRepository.FindCategoryByIDAsync(model.Id)
                : null;
            if (category == null)
            {
                category = _mapper.Map<Category>(model);
                category.Id = 0;
            }
            else
            {
                _mapper.Map(model, category);
            }
            await _blogRepository.CreateOrUpdateCategoryAsync(category);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCategorySlug(
            int id,
            string slug)
        {
            var slugExisted = await _blogRepository
                .IsCategorySlugExistedAsync(id, slug);

            return slugExisted
                ? Json($"Slug '{slug}' đã được sử dụng")
                : Json(true);
        }

        private async Task PopulatePostFilterModelAsync(PostFilterModel model)
        {
            var authors = await _authorRepository.GetAuthorsAsync();

            model.AuthorList = authors.Select(a => new SelectListItem()
            {
                Text = a.FullName,
                Value = a.Id.ToString()
            });
        }
    }
}
