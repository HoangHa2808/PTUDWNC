using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class TagEditModel
    {
        public int Id { get; set; }

        [DisplayName("Tên thẻ")]
        [Required(ErrorMessage = "Tên thẻ không được để trống")]
        [MaxLength(500, ErrorMessage = "Tên thẻ tối đa 100 ký tự")]
        public string Name { get; set; }

        [DisplayName("Mô tả")]
        [Required(ErrorMessage = "Mô tả không được để trống")]
        [MaxLength(5000, ErrorMessage = "Mô tả tối đa 2000 ký tự")]
        public string Description { get; set; }


        [DisplayName("Slug")]
        [Remote(action: "VerifyAuthorSlug", controller: "Authors", areaName: "Admin",
            HttpMethod = "post", AdditionalFields = "Id")]
        public string UrlSlug { get; set; }

        //[DisplayName("Số bài viết")]
        //[Required(ErrorMessage = "Bạn chưa chọn số lượng")]
        //public string PostCount { get; set; }

    }
}
