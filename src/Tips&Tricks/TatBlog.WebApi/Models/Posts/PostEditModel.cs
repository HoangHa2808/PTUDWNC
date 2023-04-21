using TatBlog.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TatBlog.WebApi.Models.Posts
{
    public class PostEditModel
    {
        public int Id { get; set; }

        [DisplayName("Tiêu đề")]
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(500, ErrorMessage = "Tiêu đề tối đa 500 ký tự")]
        public string Title { get; set; }

        [DisplayName("Giới thiệu")]
        [Required]
        public string ShortDescription { get; set; }

        [DisplayName("Nội dung")]
        [Required]
        public string Description { get; set; }

        [DisplayName("Metadata")]
        [Required]
        public string Meta { get; set; }

        [DisplayName("Chọn hình ảnh")]
        public IFormFile ImageFile { get; set; }

        [DisplayName("Hình hiện tại")]
        public string ImageUrl { get; set; }

        [DisplayName("Xuất bản ngay")]
        public bool Published { get; set; }

        [DisplayName("Chủ đề")]
        [Required]
        public int CategoryId { get; set; }

        [DisplayName("Tác giả")]
        [Required]
        public int AuthorId { get; set; }

        [DisplayName("Từ khóa (mỗi từ 1 dòng)")]
        [Required]
        public string SelectedTags { get; set; }

        public IEnumerable<SelectListItem> AuthorList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        public List<string> GetSelectedTags()
        {
            return (SelectedTags ?? "")
            .Split(new[] { ',', ';', '\r', '\n' },
           StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        }

        public static async ValueTask<PostEditModel> BindAsync(HttpContext context)
        {
            var form = await context.Request.ReadFormAsync(); return new PostEditModel()
            {
                ImageFile = form.Files["ImageFile"],
                Id = int.Parse(form["Id"]),
                Title = form["Title"],
                ShortDescription = form["ShortDescription"],
                Description = form["Description"],
                Meta = form["Meta"],
                Published = bool.Parse(form["Published"]),
                CategoryId = int.Parse(form["CategoryId"]),
                AuthorId = int.Parse(form["AuthorId"]),
                SelectedTags = form["SelectedTags"]
            };
        }
    }
}