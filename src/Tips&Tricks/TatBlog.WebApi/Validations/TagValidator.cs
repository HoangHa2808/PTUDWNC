﻿using FluentValidation;
using TatBlog.WebApi.Models.Tags;

namespace TatBlog.WebApi.Validations
{
    //  Cài đặt các quy tắc kiểm tra dữ liệu nhập về thông tin tác giả
    public class TagValidator : AbstractValidator<TagEditModel>
    {
        public TagValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .WithMessage("Tên thẻ không được để trống")
                .MaximumLength(100)
                .WithMessage("Tên tác giả tối đa 100 ký tự");

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