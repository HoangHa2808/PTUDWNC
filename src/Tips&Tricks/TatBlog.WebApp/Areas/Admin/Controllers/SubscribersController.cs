using Microsoft.AspNetCore.Mvc;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class SubscribersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
