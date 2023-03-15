using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.Services.Authors;
using TatBlog.WebApp.Extensions;
using TatBlog.WebApp.Mapsters;
using TatBlog.WebApp.Validations;

namespace TatBlog.WinApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            {
                // Thêm các dịch vụ được yêu cầu bởi MVC Framework
                builder.Services.AddControllersWithViews();

                //Đăng ký các dịch vụ với DI Container
                builder.Services.AddDbContext<BlogDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddScoped<IBlogRepository, BlogRepository>();
                builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
                builder.Services.AddScoped<IDataSeeder, DataSeeder>();

                builder.ConfigureMvc()
                       .ConfigureNLog()
                       .ConfigureServices()
                       .ConfigureMapster()
                       .ConfigureFluentValidation();
            }

            var app = builder.Build();
            {
                // Cấu hình HTTP Request pipeline

                // Thêm middleware để hiển thị thông báo lỗi
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Blog/Error");

                    // Thêm middleware cho việc áp dụng HSTS(thêm header
                    // Strict-Transport-Security vào HTTP Response).
                    app.UseHsts();
                }

                // Thêm middleware dể chuyển hướng HTTP sang HTTPS
                app.UseHttpsRedirection();

                // Thêm middleware phục vụ các yêu cầu liên quan
                // tới các tập tin nội dung tĩnh như hình ảnh, css, ...
                app.UseStaticFiles();

                // Thêm middleware lựa chọn endpoint phù hợp nhất
                // dể xử lý một HTTP request.
                app.UseRouting();

                //Định nghĩa route template, route cóntraint cho các endpoints
                //kết hợp với các action trong các controller
                app.MapControllerRoute(
                    name: "posts-by-category",
                    pattern: "blog/category/{slug}",
                    defaults: new { controller = "Blog", action = "Category" });

                app.MapControllerRoute(
                    name: "posts-by-tag",
                    pattern: "blog/tag/{slug}",
                    defaults: new { controller = "Blog", action = "Tag" });

                app.MapControllerRoute(
                   name: "posts-by-post",
                   pattern: "blog/post/{year:int}/{month:int}/{day:int}/{slug}",
                   defaults: new { controller = "Blog", action = "Post" });

                app.MapControllerRoute(
                name: "admin-area",
                pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}",
                defaults: new { area = "Admin" });

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Blog}/{action=Index}/{id?}");

                //Thêm dữ liệu mẫu vào CSDL
                using (var scope = app.Services.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
                    seeder.Initialize();
                }
            }
            app.Run();
        }
    }
}