using FluentValidation;
using TatBlog.WebApi.Models.Categories;

namespace TatBlog.WebApi.Validations
{
    public class CategoryValidator : AbstractValidator<CategoryEditModel>
    {
        public CategoryValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .WithMessage("Tên chủ đề không được để trống")
                .MaximumLength(100)
                .WithMessage("Tên chủ đề tối đa 100 ký tự");

            RuleFor(a => a.UrlSlug)
                .NotEmpty()
                .WithMessage("UrlSlug không được để trống")
                .MaximumLength(100)
                .WithMessage("UrlSlug tối đa 100 ký tự");

            RuleFor(a => a.Description)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống")
                .MaximumLength(100)
                .WithMessage("Mô tả tối đa 100 ký tự");
        }
    }
}