using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.Services.Subscribers;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class SubscribersController : Controller
    {
        private readonly ILogger<SubscribersController> _logger;
        private readonly IValidator<SubscriberEditModel> _commentValidator;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IMediaManager _mediaManager;
        private readonly IMapper _mapper;

        public SubscribersController(
            ILogger<SubscribersController> logger,
            IValidator<SubscriberEditModel> commentValidator,
            ISubscriberRepository subscriberRepository,
            IMediaManager mediaManager,
            IMapper mapper)
        {
            _logger = logger;
            _commentValidator = commentValidator;
            _mediaManager = mediaManager;
            _subscriberRepository = subscriberRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
            var model = await _subscriberRepository.GetPagedSubscriberAsync(pageNumber, pageSize);

            return View(model);
        }
    }
}