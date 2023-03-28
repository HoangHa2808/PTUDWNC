using Microsoft.EntityFrameworkCore;
using NLog.Web;
using System.Runtime.CompilerServices;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Authors;
using TatBlog.Services.Blogs;
using TatBlog.Services.Subscribers;
using TatBlog.Services.Media;
using TatBlog.WebApp.Extensions;
using TatBlog.WebApp.Middlewares;
using TatBlog.WinApp;

namespace TatBlog.WebApp.Extensions
{
    public static class WebApplicationExtensions
    {
        // Thêm các dịch vụ được yêu cầu bởi MVC Framework
        public static WebApplicationBuilder ConfigureMvc(
            this WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews();
            builder.Services.AddResponseCompression();

            return builder;
        }

        public static WebApplicationBuilder ConfigureNLog(
            this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            return builder;
        }

        //Đăng ký các dịch vụ với DI Container
        public static WebApplicationBuilder ConfigureServices(
               this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<BlogDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.
                GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IMediaManager, LocalFileSystemMediaManager>();
            builder.Services.AddScoped<IBlogRepository, BlogRepository>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();
            builder.Services.AddScoped<IDataSeeder, DataSeeder>();

            return builder;
        }

        //Cấu hình HTTP Request pipeline
        public static WebApplication UseRequestPipeline(
            this WebApplication app)
        {
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

            app.UseMiddleware<UserActivityMiddleware>();

            return app;
        }

        //Thêm dữ liệu mẫu vào CSDL
        public static IApplicationBuilder UseDataSeeder(
            this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
                try
                {
                    scope.ServiceProvider
                            .GetRequiredService<IDataSeeder>()
                    .Initialize();
                }
                catch (Exception ex)
                {
                    scope.ServiceProvider
                        .GetRequiredService<ILogger<Program>>()
                        .LogError(ex, "Could not insert data into database");
                }
            return app;
        }
    }
}