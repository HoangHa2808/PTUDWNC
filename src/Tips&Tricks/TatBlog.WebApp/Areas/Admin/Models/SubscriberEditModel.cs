using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class SubscriberEditModel
    {
        public int Id { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [MaxLength(500, ErrorMessage = "Email tối đa 500 ký tự")]
        public string Email{ get; set; }

        [DisplayName("Ngày đăng ký")]
        [Required(ErrorMessage = "Ngày đăng ký không được để trống")]
        public DateTime SubscribedDate { get; set; }

        [DisplayName("Ngày huỷ đăng ký")]
        [Required(ErrorMessage = "Ngày huỷ đăng ký không được để trống")]
        public DateTime UnsubscribedDate { get; set; }

        [DisplayName("Lý do")]
        [Required(ErrorMessage = "Lý do không được để trống")]
        [MaxLength(5000, ErrorMessage = "Lý do tối đa 5000 ký tự")]
        public string Reasons { get; set; }

        [DisplayName("Chú thích")]
        [Required(ErrorMessage = "Chú thích không được để trống")]
        [MaxLength(5000, ErrorMessage = "Chú thích tối đa 5000 ký tự")]
        public string Notes { get; set; }
    }
}
