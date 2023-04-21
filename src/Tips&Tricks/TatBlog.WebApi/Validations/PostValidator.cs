using FluentValidation;
using TatBlog.WebApi.Models.Posts;

namespace TatBlog.WebApi.Validations
{
    public class PostValidator : AbstractValidator<PostEditModel>
    {
        public PostValidator()
        {
            RuleFor(d => d.Title)
               .NotEmpty()
               .WithMessage("Bạn không được để trống tiêu đề bài viết")
               .MaximumLength(500)
               .WithMessage("Tiêu đề không được nhiều hơn 500 ký tự");

            RuleFor(d => d.ShortDescription)
                .NotEmpty()
                .WithMessage("Bạn không được để trống giới thiệu");

            RuleFor(d => d.Description)
                .NotEmpty()
                .WithMessage("Bạn không được để trống nội dung");

            RuleFor(d => d.Meta)
                .NotEmpty()
                .WithMessage("Bạn không được để trống metadata")
                .MaximumLength(1000)
                .WithMessage("Metadata không được nhiều hơn 100 ký tự");

            //RuleFor(d => d.UrlSlug)
            //    .NotEmpty()
            //    .WithMessage("Bạn không được để trống slug")
            //    .MaximumLength(1000)
            //    .WithMessage("Slug không được nhiều hơn 100 ký tự");

            //RuleFor(d => d.PostedDate)
            //    .GreaterThan(DateTime.MinValue)
            //    .WithMessage("Ngày đăng bài không hợp lệ");

            //RuleFor(d => d.ModifiedDate)
            //    .GreaterThan(DateTime.MinValue)
            //    .WithMessage("Ngày sửa bài không hợp lệ");

            RuleFor(d => d.CategoryId)
               .NotEmpty()
               .WithMessage("Bạn phải chọn chủ đề cho bài viết");

            RuleFor(d => d.AuthorId)
                .NotEmpty()
                .WithMessage("Bạn phải chọn tác giả của bài viết");

            //RuleFor(d => d.Tags)
            //   .NotEmpty()
            //   .WithMessage("Bạn phải nhập ít nhất một thẻ");
        }
    }
}