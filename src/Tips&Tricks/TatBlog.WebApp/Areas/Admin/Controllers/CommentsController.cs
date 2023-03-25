using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.Entities;
using TatBlog.Services.Authors;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class CommentsController : Controller
     {
        private readonly ILogger<PostsController> _logger;
        private readonly IValidator<CommentEditModel> _commentValidator;
        private readonly IBlogRepository _blogRepository;
        private readonly IMediaManager _mediaManager;
        private readonly IMapper _mapper;

        public CommentsController(
            ILogger<PostsController> logger,
            IValidator<CommentEditModel> commentValidator,
            IBlogRepository blogRepository,
            IMediaManager mediaManager,
            IMapper mapper)
        {
            _logger = logger;
            _commentValidator = commentValidator;
            _mediaManager = mediaManager;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
            var model = await _blogRepository.GetPagedCommentAsync(pageNumber, pageSize);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var comment = id > 0
                ? await _blogRepository.GetCommentByIDAsync(id)
                : null;

            // Tạo view model từ dữ liệu của chủ đề
            var model = comment == null
                ? new CommentEditModel()
                : _mapper.Map<CommentEditModel>(comment);

            // Gán các giá trị khác cho view model
            return View(model);
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            await _blogRepository.DeleteCommentByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Toggle(int id = 0)
        {
            await _blogRepository.ToggleApprovedFlagAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            CommentEditModel model)
        {
            var validationResult = await _commentValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);

            }
            var comment = model.Id > 0
                ? await _blogRepository.GetCommentByIDAsync(model.Id)
                : null;
            if (comment == null)
            {
                comment = _mapper.Map<Comment>(model);
                comment.Id = 0;
            }
            else
            {
                _mapper.Map(model, comment);
            }
            await _blogRepository.CreateOrUpdateCommentAsync(comment);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCommentSlug(
            int id,
            string slug)
        {
            var slugExisted = await _blogRepository
                .IsCommentExistedAsync(id, slug);

            return slugExisted
                ? Json($"Slug '{slug}' đã được sử dụng")
                : Json(true);
        }

    }
}
