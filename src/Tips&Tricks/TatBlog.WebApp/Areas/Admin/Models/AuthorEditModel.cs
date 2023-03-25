using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class AuthorEditModel
    {
        public int Id { get; set; }

        [DisplayName("Họ Tên tác giả")]
        [Required(ErrorMessage = "Họ Tên tác giả không được để trống")]
        [MaxLength(500, ErrorMessage = "Họ Tên tác giả tối đa 100 ký tự")]
        public string FullName { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [MaxLength(2000, ErrorMessage = "Email tối đa 2000 ký tự")]
        public string Email { get; set; }

        [DisplayName("Chú thích")]
        [Required(ErrorMessage = "Chú thích không được để trống")]
        [MaxLength(5000, ErrorMessage = "Chú thích tối đa 5000 ký tự")]
        public string Notes { get; set; }

        [DisplayName("Slug")]
        [Remote(action: "VerifyAuthorSlug", controller: "Authors", areaName: "Admin",
            HttpMethod = "post", AdditionalFields = "Id")]
        public string UrlSlug { get; set; }


        [DisplayName("Chọn hình ảnh")]
        public IFormFile ImageFile { get; set; }

        [DisplayName("Hình hiện tại")]
        public string ImageUrl { get; set; }

     
        //[DisplayName("Số bài viết")]
        //[Required(ErrorMessage = "Bạn chưa chọn số lượng")]
        //public string PostCount { get; set; }

    }
}
