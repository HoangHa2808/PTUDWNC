using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Services.Authors;
using TatBlog.Services.Blogs;
using TatBlog.Services.Subscribers;
using TatBlog.WebApi.Models;
using TatBlog.WebApi.Models.Dashboard;

namespace TatBlog.WebApi.Endpoints
{
    public static class DashboardEndpoints
    {
        public static WebApplication MapDashboardEndpoints(
      this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/dashboard");
            // Định nghĩa API endpoint đầu tiên

            routeGroupBuilder.MapGet("/", GetDashboards)
             .WithName("GetDashboards")
                .Produces<ApiResponse<Dashboards>>();
            return app;
        }

        // Tổng số bài viết, số bài viết chưa xuất bản,
        // số lượng chủ đề, số lượng tác giả, số lượng bình luận đang chờ phê duyệt,
        // số lượng người theo dõi, số lượng người mới theo dõi đăng ký (lấy số liệu trong ngày).

        private static async Task<IResult> GetDashboards(
            IAuthorRepository authorRepository,
            IBlogRepository blogRepository,
            ISubscriberRepository subscriberRepository)
        {
            var CountPost = await blogRepository.CountPostAsync();
            var CountAuthor = await authorRepository.CountAuthorAsync();
            var CountCategory = await blogRepository.CountCategoryAsync();
            var CountUnPublicPost = await blogRepository.CountUnPublicPostAsync();
            var CountComment = await blogRepository.CountCommentAsync();
            var CountSubscriber = await subscriberRepository.CountSubAsync();
            var CountSubscriberState = await subscriberRepository.CountSubscriberStateAsync();

            var dashboard = new Dashboards()
            {
                TotalPosts = CountPost,
                TotalAuthors = CountAuthor,
                TotalCategories = CountCategory,
                TotalUnpublishedPosts = CountUnPublicPost,
                TotalUnapprovedComments = CountComment,
                TotalSubscribers = CountSubscriber,
                TotalNewSubscriberToday = CountSubscriberState
            };
            return Results.Ok(ApiResponse.Success(dashboard));
        }
    }
}