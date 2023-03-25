using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.Entities;
using TatBlog.Services.Authors;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class TagsController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IValidator<TagEditModel> _tagValidator;
        private readonly IBlogRepository _blogRepository;
        private readonly IMediaManager _mediaManager;
        private readonly IMapper _mapper;

        public TagsController(
            ILogger<PostsController> logger,
            IValidator<TagEditModel> tagValidator,
            IBlogRepository blogRepository,
            IMediaManager mediaManager,
            IMapper mapper)
        {
            _logger = logger;
            _tagValidator = tagValidator;
            _mediaManager = mediaManager;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
            var model = await _blogRepository.GetPagedTagAsync(pageNumber, pageSize);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var tag = id > 0
                ? await _blogRepository.GetTagByIdAsync(id)
                : null;

            // Tạo view model từ dữ liệu của chủ đề
            var model = tag == null
                ? new TagEditModel()
                : _mapper.Map<TagEditModel>(tag);

            return View(model);
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            await _blogRepository.DeleteTagByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
           TagEditModel model)
        {
            var validationResult = await _tagValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }

            var tag = model.Id > 0
                ? await _blogRepository.GetTagByIdAsync(model.Id)
                : null;

            if (tag == null)
            {
                tag = _mapper.Map<Tag>(model);
                tag.Id = 0;
            }
            else
            {
                _mapper.Map(model, tag);
            }
            
            await _blogRepository.CreateOrUpdateTagAsync(
                tag);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyAuthorSlug(
            int id,
            string slug)
        {
            var slugExisted = await _blogRepository.IsTagSlugExistedAsync(id, slug);

            return slugExisted
                ? Json($"Slug '{slug}' đã được sử dụng")
                : Json(true);
        }
    }
}
