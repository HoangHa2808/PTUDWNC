using Microsoft.AspNetCore.Mvc;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class CommentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
