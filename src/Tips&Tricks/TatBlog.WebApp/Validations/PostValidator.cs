using FluentValidation;
using System.Reflection;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations;

public class PostValidator : AbstractValidator<PostEditModel>
{
    private readonly IBlogRepository _blogRepository;

    public PostValidator(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Bạn phải thêm tiêu đề cho bài viết")
            .MaximumLength(500)
            .WithMessage("Tiêu đề quá dài !!!!!");

        RuleFor(x => x.ShortDescription)
            .NotEmpty()
            .WithMessage("Bạn phải thêm giới thiệu cho bài viết");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Bạn phải thêm nội dung cho bài viết");

        RuleFor(x => x.Meta)
            .NotEmpty()
            .WithMessage("Bạn phải thêm phiên bản cho bài viết")
            .MaximumLength(1000)
            .WithMessage("Ký tự quá dài !!!");

        RuleFor(x => x.UrlSlug)
            .NotEmpty()
            .WithMessage("Bạn phải thêm tên định danh cho bài viết")
            .MaximumLength(1000)
            .WithMessage("Tên định danh quá dài !!!");

        RuleFor(x => x.UrlSlug)
            .MustAsync(async (postModel, slug, cancellationToken) =>
                !await blogRepository.IsPostSlugExistedAsync(
                    postModel.Id, slug, cancellationToken))
            .WithMessage("Slug '{PropertyValue}' đã được sử dụng");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Bạn phải chọn chủ đề cho bài viết");

        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("Bạn phải chọn tác giả cho bài viết");

        RuleFor(x => x.SelectedTags)
            .NotEmpty()
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
                .MustAsync(SetImageIfNotExist)
                .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
        });
    }

    // Kiểm tra xem người dùng đã nhập ít nhất 1 thẻ (tag)
    private bool HasAtLeastOneTag(
        PostEditModel postModel, string selectedTags)
    {
        return postModel.GetSelectedTags().Any();
    }

    // Kiểm tra xem bài viết đã có hình ảnh chưa.
    // Nếu chưa có, bắt buộc người dùng phải chọn file.
    private async Task<bool> SetImageIfNotExist(
        PostEditModel postModel,
        IFormFile imageFile, CancellationToken cancellationToken)
    {
        // Lấy thông tin bài viết từ CSDL
        var post = await _blogRepository.GetPostByIdAsync(
            postModel.Id, false, cancellationToken);

        // Nếu bài viết đã có hình ảnh => không bắt buộc chọn file
        if (!string.IsNullOrWhiteSpace(post?.ImageUrl))
            return true;

        // Ngược lại (bài viết chưa có hình ảnh), kiểm tra xem
        // người dùng đã chọn file hay chưa. Nếu chưa thì báo lỗi
        return imageFile is { Length: > 0 };
    }
}