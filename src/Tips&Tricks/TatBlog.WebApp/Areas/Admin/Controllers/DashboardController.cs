using Microsoft.AspNetCore.Mvc;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
