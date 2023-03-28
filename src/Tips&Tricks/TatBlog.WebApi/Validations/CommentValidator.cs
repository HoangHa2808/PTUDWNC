using FluentValidation;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations
{
    public class CommentValidator : AbstractValidator<CommentEditModel>
    {
        public CommentValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .WithMessage("Tên tác giả không được để trống")
                .MaximumLength(100)
                .WithMessage("Tên tác giả tối đa 100 ký tự");

            RuleFor(a => a.Description)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống")
                .MaximumLength(100)
                .WithMessage("Mô tả tối đa 100 ký tự");

            RuleFor(a => a.PostedDate)
                .GreaterThan(DateTime.MinValue)
                .WithMessage("Ngày tham gia không hợp lệ");

            RuleFor(a => a.PostId)
                .NotEmpty()
                .WithMessage("Mã bài viết không được để trống")
                .MaximumLength(100)
                .WithMessage("Mã bài viết tối đa 5 ký tự");
        }
    }
}