using FluentValidation;
using System.Collections.Generic;
using System.Threading;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class PostValidator : AbstractValidator<PostEditModel>
    {
        private readonly IBlogRepository _blogRepository;

        public PostValidator(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
            RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

            RuleFor(x => x.ShortDescription)
            .NotEmpty();

            RuleFor(x => x.Description)
            .NotEmpty();

            RuleFor(x => x.Meta)
            .NotEmpty()
            .MaximumLength(1000);

            RuleFor(x => x.UrlSlug)
            .NotEmpty()
            .MaximumLength(1000);

            RuleFor(x => x.UrlSlug)
            .MustAsync(async (postModel, slug, cancellationToken) =>
            !await blogRepository.IsPostSlugExistedAsync(
            postModel.Id, slug, cancellationToken))
            .WithMessage("Slug '{Propertyvalue}' đã được sử dụng");

            RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Bạn phải chọn chủ đề cho bài viết");

            RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("Bạn phải chọn tác giả cho bài viết");

            RuleFor(x => x.SelectedTags)
            .Must(HasAtLeastOneTags)
            .WithMessage("Bạn phải nhập ít nhất một thẻ");

            When(x => x.Id <= 0, () =>
            {
                RuleFor(x => x.ImageFile)
                    .Must(x => x is { Length: > 0 })
                    .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
            })
            .Otherwise(() =>
{
    RuleFor(x => x.ImageFile)
        .MustAsync(SetImageIfNotExits)
        .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
});
        }

        // Kiểm tra xem người dùng đã nhập ít nhất 1 thẻ(tag)
        private bool HasAtLeastOneTags(
        PostEditModel postModel, string selectedTags)
        {
            return postModel.GetSelectedTags().Any();
        }
        // Kiểm tra xem bài viết có hình ảnh chưa
        // Nếu chưa có, bắt buộc người dùng phải chọn file.
        private async Task<bool> SetImageIfNotExits(
        PostEditModel postModel,
        IFormFile imageFile,
        CancellationToken cancellationToken)
        {
            // Lay thong tin bai viet tu CSDL
            var post = await _blogRepository.GetPostByIdAsync(
            postModel.Id, false, cancellationToken);

            // Nếu bài viết đã có hình ảnh => Không bắt buộc chọn file
            if (!string.IsNullOrWhiteSpace(post?.ImageUrl))
                return true;
            // Ngược lại (bài viết chưa có hình ảnh), Kiểm tra xem
            // người dùng đã chọn file hay chưa. Nếu chưa thì báo lỗi.
            return imageFile is { Length: > 0 };
        }
    }
}
