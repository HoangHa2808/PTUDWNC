﻿namespace TatBlog.WebApi.Models
{
    public class PostDTO
    {
        // Mã bài viết
        public int Id { get; set; }

        // Tiêu đề bài viết
        public string Title { get; set; }

        // Mô tả hay giới thiệu ngắn về nội dung
        public string ShortDescription { get; set; }

        // Tên định danh để tạo URL
        public string UrlSlug { get; set; }

        // Đường dẫn đến tập tin hình ảnh
        public string ImageUrl { get; set; }

        // Số lượt xem, đọc bài viết
        public int ViewCount { get; set; }

        // Ngày giờ đăng
        public DateTime PostedDate { get; set; }

        // Ngày giờ cập nhật lần cuối
        public DateTime? ModifiedDate { get; set; }

        // Chuyên mục của bài viết
        public CategoryDTO Category { get; set;}

        // Tác giả của bài viết
        public AuthorDTO Author { get; set;}

        // Danh sách các từ khoá của bài viết
        public IList<TagDTO> Tags { get; set;}
    }
}
