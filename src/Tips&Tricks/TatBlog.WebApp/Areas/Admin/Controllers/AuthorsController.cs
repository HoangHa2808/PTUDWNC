using Microsoft.AspNetCore.Mvc;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class AuthorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
