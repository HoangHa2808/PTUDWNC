using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class CategoryEditModel
    {
        public int Id { get; set; }

        [DisplayName("Tên chủ đề")]
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(500, ErrorMessage = "Tiêu đề tối đa 500 ký tự")]
        public string Name { get; set; }

        [DisplayName("Nội dung")]
        [Required(ErrorMessage = "Nội dung không được để trống")]
        [MaxLength(5000, ErrorMessage = "Nội dung tối đa 5000 ký tự")]
        public string Description { get; set; }

        [DisplayName("Slug")]
        [Required(ErrorMessage = "Slug không được để trống")]
        [MaxLength(200, ErrorMessage = "Slug tối đa 200 ký tự")]
        public string UrlSlug { get; set; }

        [DisplayName("Chủ đề")]
        [Required(ErrorMessage = "Bạn chưa chọn chủ đề")]
        public int CategoryId { get; set; }

        [DisplayName("Tác giả")]
        [Required(ErrorMessage = "Bạn chưa chọn tác giả")]
        public int AuthorId { get; set; }

        [DisplayName("Xuất bản")]
        public bool ShowOnMenu { get; set; }

        [DisplayName("Từ khoá (mỗi từ 1 dòng)")]
        [Required(ErrorMessage = "Bạn chưa nhập tên thẻ")]
        public string SelectedTags { get; set; }

        public IEnumerable<SelectListItem> AuthorList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        // Tách chuỗi chứa các thẻ thành một mảng các chuỗi
        public List<string> GetSelectedTags()
        {
            return (SelectedTags ?? "")
                 .Split(new[] { ',', ';', '\r', '\n' },
                     StringSplitOptions.RemoveEmptyEntries)
                 .ToList();
        }
    }
}
