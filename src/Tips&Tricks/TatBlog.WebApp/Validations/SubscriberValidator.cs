using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class SubscriberValidator : AbstractValidator<SubscriberEditModel>
    {
        private readonly IBlogRepository _blogRepository;

        public SubscriberValidator(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Bạn phải thêm email khi đăng ký")
                .MaximumLength(500)
                .WithMessage("Chủ đề quá dài !!!!!");

            RuleFor(x => x.Reasons)
                .NotEmpty()
                .WithMessage("Bạn phải thêm lý do khi huỷ đăng ký")
                .MaximumLength(1000)
                .WithMessage("Chủ đề quá dài !!!!!");
         
        }

    }
}