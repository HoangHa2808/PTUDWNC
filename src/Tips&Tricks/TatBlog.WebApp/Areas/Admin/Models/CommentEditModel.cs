using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class CommentEditModel
    {
        public int Id { get; set; }

        [DisplayName("Tên bình luận")]
        [Required(ErrorMessage = "Tên bình luận không được để trống")]
        [MaxLength(500, ErrorMessage = "Tên bình luận tối đa 500 ký tự")]
        public string Name { get; set; }

        [DisplayName("Nội dung")]
        [Required(ErrorMessage = "Nội dung không được để trống")]
        [MaxLength(5000, ErrorMessage = "Nội dung tối đa 5000 ký tự")]
        public string Description { get; set; }

        [DisplayName("Chấp thuận")]
        public bool IsApproved { get; set; }

    }
}
