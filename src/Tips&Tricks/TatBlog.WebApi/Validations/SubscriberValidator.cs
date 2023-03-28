using FluentValidation;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations
{
    public class SubscriberValidator : AbstractValidator<SubscriberEditModel>
    {
        public SubscriberValidator()
        {
            RuleFor(a => a.Email)
                .NotEmpty()
                .WithMessage("Email không được để trống")
                .MaximumLength(100)
                .WithMessage("Email tối đa 100 ký tự");

            RuleFor(a => a.SubscribedDate)
                .GreaterThan(DateTime.MinValue)
                .WithMessage("Ngày đăng ký không hợp lệ");

            RuleFor(a => a.UnsubscribedDate)
                .GreaterThan(DateTime.MinValue)
                .WithMessage("Ngày huỷ đăng ký không hợp lệ");

            RuleFor(a => a.Reasons)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống")
                .MaximumLength(100)
                .WithMessage("Mô tả tối đa 100 ký tự");

            RuleFor(a => a.Notes)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống")
                .MaximumLength(100)
                .WithMessage("Mô tả tối đa 100 ký tự");
        }
    }
}