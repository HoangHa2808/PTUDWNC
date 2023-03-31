using TatBlog.Core.Entities;

namespace TatBlog.WebApi.Models.Dashboard
{
    public class Dashboards
    {
        // Tổng số bài viết, số bài viết chưa xuất bản,
        // số lượng chủ đề, số lượng tác giả, số lượng bình luận đang chờ phê duyệt,
        // số lượng người theo dõi, số lượng người mới theo dõi đăng ký (lấy số liệu trong ngày).
        public int TotalPosts { get; set; }

        public int TotalAuthors { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUnpublishedPosts { get; set; }
        public int TotalUnapprovedComments { get; set; }
        public int TotalSubscribers { get; set; }
        public int TotalNewSubscriberToday { get; set; }
    }
}