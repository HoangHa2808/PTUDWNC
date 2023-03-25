using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class CategoryValidator : AbstractValidator<CategoryEditModel>
    {
        private readonly IBlogRepository _blogRepository;

        public CategoryValidator(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Bạn phải thêm chủ đề cho bài viết")
                .MaximumLength(500)
                .WithMessage("Chủ đề quá dài !!!!!");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Bạn phải thêm giới thiệu cho bài viết");

            RuleFor(x => x.UrlSlug)
                .NotEmpty()
                .WithMessage("Bạn phải thêm tên định danh cho bài viết")
                .MaximumLength(1000)
                .WithMessage("Tên định danh quá dài !!!");

            RuleFor(x => x.UrlSlug)
                .MustAsync(async (categoryModel, slug, cancellationToken) =>
                    !await blogRepository.IsCategorySlugExistedAsync(
                        categoryModel.Id, slug, cancellationToken))
                .WithMessage("Slug '{PropertyValue}' đã được sử dụng");

        }

    }
}