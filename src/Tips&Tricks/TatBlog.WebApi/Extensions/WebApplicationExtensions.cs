using Microsoft.EntityFrameworkCore;
using NLog.Web;
using System.Runtime.CompilerServices;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Timing;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Extensions;

namespace TatBlog.WebApi.Extensions
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

        public static WebApplicationBuilder ConfigureSwaggerOpenApi(
        this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            return builder;
        }

        // Cấu hình việc sử dụng NLog
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
            builder.Services.AddMemoryCache();
            builder.Services.AddDbContext<BlogDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.
                GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<ITimeProvider, LocalTimeProvider>();
            builder.Services.AddScoped<IMediaManager, LocalFileSystemMediaManager>();
            builder.Services.AddScoped<IBlogRepository, BlogRepository>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<IDataSeeder, DataSeeder>();

            return builder;
                    }

        public static WebApplicationBuilder ConfigureCors(
               this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("TatBlogApp", policyBuilder =>
                policyBuilder
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod());
            });
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

            //app.UseMiddleware<UserActivityMiddleware>();

            return app;
        }

        //Thêm dữ liệu mẫu vào CSDL
        public static WebApplication SetupRequestPipeline(
            this WebApplication app)
        {
           if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseCors("TatBlogApp");

            return app;
        }
    }
}
