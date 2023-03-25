using Microsoft.AspNetCore.Mvc;
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
        [Remote(action: "VerifyCategorySlug", controller: "Categories", areaName: "Admin",
            HttpMethod = "post", AdditionalFields = "Id")]
        public string UrlSlug { get; set; }

        [DisplayName("Xuất bản")]
        public bool ShowOnMenu { get; set; }

    }
}
