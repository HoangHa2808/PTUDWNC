using FluentValidation;
using TatBlog.Services.Authors;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class TagValidator : AbstractValidator<TagEditModel>
    {
        private readonly IBlogRepository _blogRepository;

        public TagValidator(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Bạn phải thêm đầy đủ họ tên cho thẻ")
                .MaximumLength(500)
                .WithMessage("Chủ đề quá dài !!!!!");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Bạn phải thêm mô tả cho thẻ")
                .MaximumLength(1000)
                .WithMessage("Chủ đề quá dài !!!!!");

            RuleFor(x => x.UrlSlug)
                .NotEmpty()
                .WithMessage("Bạn phải thêm tên định danh cho thẻ")
                .MaximumLength(1000)
                .WithMessage("Tên định danh quá dài !!!");

            RuleFor(x => x.UrlSlug)
                .MustAsync(async (tagModel, slug, cancellationToken) =>
                    !await blogRepository.IsTagSlugExistedAsync(
                        tagModel.Id, slug, cancellationToken))
                .WithMessage("Slug '{PropertyValue}' đã được sử dụng");

        }

    }
}