using TatBlog.Core.Contracts;
using TatBlog.Core.Entities;
using TatBlog.Core.DTO;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;



namespace TatBlog.WinApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Tạo đối tượng context để quản lý phiên làm việc
            var context = new BlogDbContext();
            InitDB(context);

            #region Phần B
            //XuatDanhSachTacGia(context);
            //XuatDanhSachBaiViet(context);
            //BaiVietDuocXemNhieuNhat(context, 3);
            //XuatDanhSachDanhMuc(context);
            //XuatDanhSachTheTheoPhanTrang(context);
            #endregion

            #region Phần C
            //TimMotTheTagTheoSlug(context, "tag-7");
            //XuatDanhSachTatCaCacThe(context);
            //XoaTags(context, 2);
            //TimMotChuyenMucTheoSlug(context, "");
            TimMotChuyenMucTheoID(context, 3);
            //ThemHoacCapNhatMotChuyenMuc(context);
            //XoaCategories(context, 2);

            #endregion

            // Wait
            Console.ReadKey();
        }

        static void InitDB(BlogDbContext context)
        {
            // Tạo đối tượng khởi tạo dữ liệu mẫu
            var seeder = new DataSeeder(context);
            // Gọi hàm nhập dữ liệu mẫu
            seeder.Initialize();
        }

        #region Phần B.Hướng dẫn
        static void XuatDanhSachTacGia(BlogDbContext context)
        {
            // Đọc danh sách tác giả từ CSDL
            var authors = context.Authors.ToList();

            // Xuất danh sách tác giả ra màn hình
            Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12}", "ID", "Full Name", "Email", "Joined Date");
            foreach (var author in authors)
            {
                Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12:MM/dd/yyyy}",
                    author.Id, author.FullName, author.Email, author.JoinedDate);
            }
        }

        static void XuatDanhSachBaiViet(BlogDbContext context)
        {
            var posts = context.Posts.Where(p => p.Published).OrderBy(p => p.Title).Select(p => new
            {
                Id = p.Id,
                Title = p.Title,
                ViewCount = p.ViewCount,
                PostedDate = p.PostedDate,
                Author = p.Author.FullName,
                Category = p.Category.Name
            }).ToList();

            foreach (var post in posts)
            {
                Console.WriteLine("ID      : {0}", post.Id);
                Console.WriteLine("Title   : {0}", post.Title);
                Console.WriteLine("View    : {0}", post.ViewCount);
                Console.WriteLine("Date    : {0:MM/dd/yyyy}", post.PostedDate);
                Console.WriteLine("Author  : {0}", post.Author);
                Console.WriteLine("Category: {0}", post.Category);
                Console.WriteLine("".PadRight(80, '-'));
            }
        }

        static async void BaiVietDuocXemNhieuNhat(BlogDbContext context, int numPost)
        {
            // Tạo đối tượng BlogRepository
            IBlogRepository blogRepo = new BlogRepository(context);

            var posts = await blogRepo.GetPopularArticlesAsync(numPost);

            foreach (var post in posts)
            {
                Console.WriteLine("ID      : {0}", post.Id);
                Console.WriteLine("Title   : {0}", post.Title);
                Console.WriteLine("View    : {0}", post.ViewCount);
                Console.WriteLine("Date    : {0:MM/dd/yyyy}", post.PostedDate);
                Console.WriteLine("Author  : {0}", post.Author);
                Console.WriteLine("Category: {0}", post.Category);
                Console.WriteLine("".PadRight(80, '-'));
            }
        }

        static async void XuatDanhSachDanhMuc(BlogDbContext context)
        {
            // Tạo đối tượng BlogRepository
            IBlogRepository blogRepo = new BlogRepository(context);

            var categories = await blogRepo.GetCategoriesAsync();

            Console.WriteLine("{0,-5}{1,-50}{2,10}", "ID", "Name", "Count");

            foreach (var category in categories)
            {
                Console.WriteLine("{0,-5}{1,-50}{2,10}", category.Id, category.Name, category.PostCount);
            }
        }

        static async void XuatDanhSachTheTheoPhanTrang(BlogDbContext context)
        {
            // Tạo đối tượng BlogRepository
            IBlogRepository blogRepo = new BlogRepository(context);

            // Tạo đối tượng chứa tham số phân trang
            var pagingParams = new PagingParams()
            {
                PageNumber = 1,
                PageSize = 5,
                SortColumn = "Name",
                SortOrder = "DESC"
            };

            // Lấy danh sách từ khóa
            var tagsList = await blogRepo.GetPagedTagsAsync(pagingParams);

            // Xuất ra màn hình
            Console.WriteLine("{0,-5}{1,-50}{2,10}", "ID", "Name", "Count");

            foreach (var tag in tagsList)
            {
                Console.WriteLine("{0,-5}{1,-50}{2,10}", tag.Id, tag.Name, tag.PostCount);
            }
        }
        #endregion

        #region Phần C.1.Thực hành
        // Cau 1.a: Tìm một thẻ(Tag) theo tên định danh(slug)
        static async void TimMotTheTagTheoSlug(BlogDbContext context, string slug)
        {
            // Tạo đối tượng BlogRepository
            IBlogRepository blogRepo = new BlogRepository(context);

            Tag tag = await blogRepo.FindTagWithSlugAsync(slug);

            Console.WriteLine("{0,-5}{1,-30}{2,-10}", "ID", "Name", "Count");
            if (tag != null)
            {
                Console.WriteLine("{0,-5}{1,-30}{2,-10}", tag.Id, tag.Name, tag.Posts.Count);
            }
        }

        // Cau 1.c: Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó
        static async void XuatDanhSachTatCaCacThe(BlogDbContext context)
        {
            // Tạo đối tượng BlogRepository
            IBlogRepository blogRepo = new BlogRepository(context);

            var tags = await blogRepo.GetTagsItemsAsync();

            Console.WriteLine("{0,-5}{1,-30}{2,-10}", "ID", "Name", "Count");

            foreach (var tag in tags)
            {
                Console.WriteLine("{0,-5}{1,-30}{2,-10}", tag.Id, tag.Name, tag.PostCount);
            }
        }

        // Cau 1.d: Xóa một thẻ theo mã cho trước
        static async void XoaTags(BlogDbContext context, int id)
        {
            IBlogRepository blogRepo = new BlogRepository(context);
            await blogRepo.DeleteTagWithIdAsync(id);
            Console.ReadLine();
            var tagD = await blogRepo.GetTagsItemsAsync();
            Console.WriteLine("{0,-5}{1,-50}{2,-30}{3,-30}", "ID", "TagName", "Url", "Des");
            foreach (var tag in tagD)
            {
                Console.WriteLine("{0,-5}{1,-50}{2,-30}{3,-30}",
                  tag.Id, tag.Name, tag.UrlSlug, tag.Description);
            }
        }

        // Câu 1.e: Tìm một chuyên mục (Category) theo tên định danh(slug)
        static async void TimMotChuyenMucTheoSlug(BlogDbContext context, string slug)
        {
            // Tạo đối tượng BlogRepository
            IBlogRepository blogRepo = new BlogRepository(context);

            Category category = await blogRepo.FindCategoryByUrlAsync(slug);

            Console.WriteLine("{0,-5}{1,-30},{2,-10}", "ID", "Name", "Count");
            if (category != null)
            {
                Console.WriteLine("{0,-5}{1,-30}{2,-10}", category.Id, category.Name, category.Posts.Count);
            }
        }

        // Câu 1.f: Tìm 1 chuyên mục theo mã số
        static async void TimMotChuyenMucTheoID(BlogDbContext context, int id)
        {
            // Tạo đối tượng BlogRepository
            IBlogRepository blogRepo = new BlogRepository(context);

            Category category = await blogRepo.FindCategoryByIDAsync(id);

            Console.WriteLine("{0,-5}{1,-30},{2,-10}", "ID", "Name", "Count");
            if (category != null)
            {
                Console.WriteLine("{0,-5}{1,-30}{2,-10}", category.Id, category.Name, category.Posts.Count);
            }
        }

        // Câu 1.g: Thêm hoặc cập nhật một chuyên mục
        static async void ThemHoacCapNhatMotChuyenMuc(BlogDbContext context)
        {
            IBlogRepository blogRepo = new BlogRepository(context);

            Category category = new Category()
            {
                Id = -1,
                Name = "Ha",
                Description = "",
                UrlSlug = "",
                Posts = new List<Post>(),
                ShowOnMenu = true
            };
            await blogRepo.AddOrUpdateCategoryAsync(category);
        }

        // Câu 1.h: Xóa một chuyên mục theo mã số
        static async void XoaCategories(BlogDbContext context, int id)
        {
            IBlogRepository blogRepo = new BlogRepository(context);
            await blogRepo.DeleteCategoryByIdAsync(id);
            //Console.ReadLine();
            //var tagC = await blogRepo.GetCategoriesAsync();
            //Console.WriteLine("{0,-5}{1,-30}{2,-30}{3,-30}", "ID", "CategoriesName", "Url", "Des");
            //foreach (var tag in tagC)
            //{
            //    Console.WriteLine("{0,-5}{1,-30}{2,-30}{3,-30}",
            //      tag.Id, tag.Name, tag.UrlSlug, tag.Description);
            //}
        }

        // Câu 1.i: Kiểm tra tên định danh (slug) của
        // một chuyên mục đã tồn tại hay chưa.
        static async void KiemTraName(BlogDbContext context, int n, string name)
        {
            IBlogRepository blogRepo = new BlogRepository(context);
            Category category = new Category()
            {
                UrlSlug = "OOP"
            };
            await blogRepo.IsCategorySlugExistedAsync(n, name);
        }

        // Câu 1.j: Lấy và phân trang danh sách chuyên mục
        static async void LayVaPhanTrang(BlogDbContext context)
        {
            IBlogRepository blogRepo = new BlogRepository(context);
            var pagingParams1 = new PagingParams()
            {
                PageNumber = 1,
                PageSize = 6,
                SortColumn = "name",
                SortOrder = "desc"
            };
            var categoriesList = await blogRepo.GetPagedCategoryAsync(pagingParams1);
            Console.WriteLine("{0,-5}{1,-50}{2,10}",
        "ID", "Name", "Count");
            foreach (var item in categoriesList)
            {
                Console.WriteLine("{0,-5}{1,-50}{2,10}",
                    item.Id, item.Name, item.PostCount);
            }
        }

        // Câu 1.k: Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào.
        // Kết quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số bài viết


        // Câu 1.l: Tìm một bài viết theo mã số
        static async void TimMotBaiVietTheoID(BlogDbContext context, int id)
        {
            IBlogRepository blogRepo = new BlogRepository(context);
            Post post = await blogRepo.FindPostByIDAsync(id);

            //Console.WriteLine("{0,-5}{1,-30},{2,-10}", "ID", "Title", "Description");
            //if (post != null)
            //{
            //    Console.WriteLine("{0,-5}{1,-30}{2,-10}", post.Id, post.Title, post.Description);
            //}
            Console.WriteLine("ID       :{0}", post.Id);
            Console.WriteLine("Title    :{0}", post.Title);
            Console.WriteLine("View     :{0}", post.ViewCount);
            Console.WriteLine("Date     :{0}:MM/dd/yyyy", post.PostedDate);
            Console.WriteLine("Author   :{0}", post.Author);
            Console.WriteLine("Category :{0}", post.Category);
            Console.WriteLine("Tags :{0}", post.Tags.Count);
            Console.WriteLine("".PadRight(80, '-'));
        }

        // Câu 1.m: Thêm hay cập nhật một bài viết
        async void ThemHoacCapNhatMotBaiViet(BlogDbContext context)
        {
            IBlogRepository blogRepo = new BlogRepository(context);

            Post post = new Post()
            {
                Id = -1,
                Title = "Ha",
                ShortDescription = "",
                Description = "",
                Meta = "",
                UrlSlug = "",
                ImageUrl = "",
                ViewCount = 4,
                Published = true,
                //PostedDate = ,
                //ModifiedDate =,
                CategoryId = 2,
                AuthorId = 6

            };
            await blogRepo.AddOrUpdatePostAsync(post);
        }

        // Câu 1.n: Chuyển đổi trạng thái Published của bài viết
        async void ChuyenDoiTrangThai(BlogDbContext context)
        {
            IBlogRepository blogRepo = new BlogRepository(context);
            int id = 3;
            await blogRepo.PublishedAsync(id);
        }

        // Câu 1.o: Lấy ngẫu nhiên N bài viết. N là tham số đầu vào
        async void LayNgauNhienNPost(BlogDbContext context)
        {
            IBlogRepository blogRepo = new BlogRepository(context);
            int id = 4;
            var random = await blogRepo.GetRandomPostsAsync(id);

            foreach (var post in random)
            {
                Console.WriteLine("ID       :{0}", post.Id);
                Console.WriteLine("Title    :{0}", post.Title);
                Console.WriteLine("View     :{0}", post.ViewCount);
                Console.WriteLine("Date     :{0}:MM/dd/yyyy", post.PostedDate);
                Console.WriteLine("Author   :{0}", post.Author);
                Console.WriteLine("Category :{0}", post.Category);
                Console.WriteLine("Tags :{0}", post.Tags.Count);
                Console.WriteLine("".PadRight(80, '-'));
            }
        }

        // 1.p: Tạo lớp PostQuery để lưu trữ các điều kiện tìm kiếm bài viết. Chẳng hạn:mã tác giả,
        // mã chuyên mục, tên ký hiệu chuyên mục, năm/tháng đăng bài, từ khóa, … 
        static async void TaoLopPostQuery(BlogDbContext context)
        {
            IBlogRepository blogRepo = new BlogRepository(context);

            PostQuery pq = new PostQuery()
            {
                TagId = 3,
                CategorySlug = "",
            };
            var listAllPost = await blogRepo.GetAllPostsByPostQuery(pq);
            foreach (var post in listAllPost)
            {
                Console.WriteLine(post.Id + "" + post.Title);
            }
        }
       
        // 1.q: Tìm tất cả bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối tượng
        // PostQuery(kết quả trả về kiểu IList<Post>)
        async void TimTatCaBaiViet(BlogDbContext context)
        {
            IBlogRepository blogRepo = new BlogRepository(context);

            PostQuery pq = new PostQuery()
            {
                AuthorId = 3
            };
            var posts = await blogRepo.FindAllPostsWithPostQueryAsync(pq);

            foreach (var post in posts)
            {
                Console.WriteLine("ID       :{0}", post.Id);
                Console.WriteLine("Title    :{0}", post.Title);
                Console.WriteLine("View     :{0}", post.ViewCount);
                Console.WriteLine("Date     :{0}:MM/dd/yyyy", post.PostedDate);
                Console.WriteLine("Author   :{0}", post.Author);
                Console.WriteLine("Category :{0}", post.Category);
                //Console.WriteLine("Tags :{0}", post.Tags.Count);
                Console.WriteLine("".PadRight(80, '-'));
            }
        }

        // Câu 1.r: Đếm số lượng bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối tượng PostQuery
        static async void DemSoLuongBaiViet(BlogDbContext context)
        {
            IBlogRepository blogRepo = new BlogRepository(context);

            PostQuery pq = new PostQuery()
            {
                AuthorId = 3
            };
            int count = await blogRepo.CountPostsWithPostQueryAsync(pq);
            Console.WriteLine("Count post: ", count);
        }
        // 1.S: Tìm và phân trang các bài viết thỏa mãn điều kiện tìm kiếm được cho trong
        // đối tượng PostQuery(kết quả trả về kiểu IPagedList<Post>)


        // 1.T: Tương tự câu trên nhưng yêu cầu trả về kiểu IPagedList<T>. Trong đó T
        // là kiểu dữ liệu của đối tượng mới được tạo từ đối tượng Post.Hàm này có
        // thêm một đầu vào là Func<IQueryable<Post>, IQueryable<T>> mapper
        // để ánh xạ các đối tượng Post thành các đối tượng T theo yêu cầu
       
        #endregion

        #region Phần C.2

        #endregion
    }
}