using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace TatBlog.WebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        #region _SearchForm
        public async Task<IActionResult> Index(
            [FromQuery(Name = "k")] string keyword = null,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {
                //Chỉ lấy những bài viết có trạng thái Published
                PublishedOnly = true,

                //Tìm bài viết theo từ khoá
                Keyword = keyword
            };

            //Truy vấn các bài viết theo điều kiện để tạo
            var postsList = await _blogRepository
                .GetPagedPostAsync(postQuery, pageNumber, pageSize);

            //Lưu lại điều kiện truy vấn để hiển thị trong View
            ViewBag.PostQuery = postQuery;
            //ViewBag.

            // Truyền danh sách bài viết vào View để render ra HTML
            return View(postsList);
        }
        #endregion


        #region Hiển thị danh sách bài viết thuộc chủ đề
        public async Task<IActionResult> Category(
            string keyword = null,
             string slug = null,
             int pageNumber = 1,
             int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {
                PublishedOnly = true,
                Keyword = keyword,
                CategorySlug = slug
            };

            var postsList = await _blogRepository
                .GetPagedPostAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;
            //ViewBag.

            return View(postsList);
        }
        #endregion


        #region Hiển thị danh sách bài viết theo tác giả
        public async Task<IActionResult> Author(
            string slugPost = null,
            string keyword = null,
             int pageNumber = 1,
             int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {
                
                PublishedOnly = true,
                Keyword = keyword,
                AuthorSlug = slugPost
            };

            var postsList = await _blogRepository
                .GetPagedPostAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;
            //ViewBag.

            return View(postsList);
        }
        #endregion


        #region Hiển thị danh sách bài viết chứa thẻ
        public async Task<IActionResult> Tag(
            string slugPost = null,
            string keyword = null,
             int pageNumber = 1,
             int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {

                PublishedOnly = true,
                Keyword = keyword,
                TagSlug = slugPost
            };

            var postsList = await _blogRepository
                .GetPagedPostAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;
            //ViewBag.

            return View(postsList);
        }
            #endregion


            #region Hiển thị chi tiết 1 bài viết khi người dùng nhấn vào
            // nút Xem chi tiết hoặc tiêu đề bài viết ở trang chủ
            public async Task<IActionResult> Post(
                string slugPost = null,
            string keyword = null,
             int pageNumber = 1,
             int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {

                PublishedOnly = true,
                Keyword = keyword
            };

            var postsList = await _blogRepository
                .GetPagedPostAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;
            //ViewBag.

            return View(postsList);
        }
        #endregion


        #region Hiển thị danh sách bài viết được đăng trong tháng và năm đã chọn
        public async Task<IActionResult> Archives(
            string slugPost,
            int year, int month, int day,
            string keyword = null,
             int pageNumber = 1,
             int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {

                PublishedOnly = true,
                Keyword = keyword,
                PostSlug = slugPost,
               
            };

            var postsList = await _blogRepository
                .GetPagedPostAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;
            //ViewBag.

            return View(postsList);
        }
        #endregion


        #region Hiển thị thông tin liên hệ, bản đồ và form để gửi ý kiến
        public IActionResult Contact()
        => View();
        
        #endregion


        #region Hiển thị trang giới thiệu về blog (nội dung tĩnh)
        public IActionResult About()
        {
            return View();
        }
        #endregion


        public IActionResult Rss()
            => Content("Nội dung sẽ được cập nhật");
    }
}
