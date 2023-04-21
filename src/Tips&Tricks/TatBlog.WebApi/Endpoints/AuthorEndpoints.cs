using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Authors;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;
using TatBlog.Core.Contracts;
using TatBlog.WebApi.Models.Authors;
using TatBlog.WebApi.Models.Posts;
using System.Net;

namespace TatBlog.WebApi.Endpoints
{
    public static class AuthorEndpoints
    {
        public static WebApplication MapAuthorEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/authors");
            // Định nghĩa API endpoint đầu tiên

            // Lấy danh sách tác giả. Hỗ trợ tìm theo tên và phân trang kết quả
            routeGroupBuilder.MapGet("/", GetAuthors)
                .WithName("GetAuthors")
                .Produces<ApiResponse<PaginationResult<AuthorItem>>>();

            // Quản lý thông tin tác giả
            // Lấy thông tin chi tiết của tác giả có mã số(id) cho trước
            routeGroupBuilder.MapGet("/{id:int}", GetAuthorDetails)
                .WithName("GetAuthorById")
                .Produces<ApiResponse<AuthorItem>>();

            // GetPostsByAuthorsSlug
            routeGroupBuilder.MapGet(
                "/{slug:regex(^[a-z0-9 -]+$)}/posts", GetPostByAuthorSlug)
                .WithName("GetPostByAuthorSlug")
                .Produces<ApiResponse<PostDTO>>();

            // ListAuthorBest
            // Lấy danh sách N (limit) tác giả có nhiều bài viết nhất
            routeGroupBuilder.MapGet("/best/{limit:int}", ListBestAuthors)
              .WithName("GetBestAuthors")
              .Produces<ApiResponse<AuthorItem>>();

            // AddAuthor
            routeGroupBuilder.MapPost("/", AddAuthor)
                .WithName("AddNewAuthor")
                .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
                .Produces(401)
              .Produces<ApiResponse<AuthorItem>>();

            // SetAuthorPicture
            // Upload một hình ảnh và sử dụng nó làm ảnh đại diện cho tác giả
            routeGroupBuilder.MapPost("/{id:int}/avatar", SetAuthorPicture)
                .WithName("SetAuthorPicture")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces(401)
                .Produces<ApiResponse<string>>();

            // UpdateAuthor
            routeGroupBuilder.MapPut("/{id:int}", UpdateAuthor)
               .WithName("UpdateAnAuthor")
               .Produces(401)
                .Produces<ApiResponse<string>>();

            // DeleteAuthor
            routeGroupBuilder.MapDelete("/{id:int}", DeleteAuthor)
                .WithName("DeleteAuthor")
                .Produces(401)
                .Produces<ApiResponse<string>>();

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách tác giả
        public static async Task<IResult> GetAuthors(
            IAuthorRepository authorRepository)
        {
            var authorList = await authorRepository
                .GetAuthorsAsync();

            return Results.Ok(ApiResponse.Success(authorList));
        }

        // GetAuthorDetails
        public static async Task<IResult> GetAuthorDetails(
           int id,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            var author = await authorRepository.GetCachedAuthorByIdAsync(id);
            return author == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy tác giả có mã số {id}"))
                : Results.Ok(ApiResponse.Success(mapper.Map<AuthorItem>(author)));
        }

        // GetPostByAuthorSlug
        public static async Task<IResult> GetPostByAuthorSlug(
            string slug,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            var author = await authorRepository.GetAuthorBySlugAsync(slug);

            return author == null ? Results.Ok(ApiResponse.Fail(
                HttpStatusCode.NotFound, $"Không tìm thấy tác giả có tên định danh {slug}"))
               : Results.Ok(ApiResponse.Success(mapper.Map<PostDetail>(author)));

        }

        // ListAuthorBest
        public static async Task<IResult> ListBestAuthors(
           int limit,
            IAuthorRepository authorRepository)
        {
            var author = await authorRepository.ListAuthorAsync(limit);
            //return author == null ? Results.NotFound($"Không tìm thấy danh sách {N} tác giả có nhiều bài viết đóng góp cho blog nhất")
            //    : Results.Ok(mapper.Map<AuthorItem>(author));
            return Results.Ok(ApiResponse.Success(author));
        }

        // AddAuthor
        private static async Task<IResult> AddAuthor(
            AuthorEditModel model,
            //IValidator<AuthorEditModel> validator,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            //var validationResult = await validator.ValidateAsync(model);

            //if (!validationResult.IsValid)
            //{
            //    return Results.BadRequest(validationResult.Errors.ToResponse());
            //}

            if (await authorRepository.IsAuthorSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                    $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var author = mapper.Map<Author>(model);
            await authorRepository.AddOrUpdateAuthorAsync(author);

            return Results.Ok(ApiResponse.Success(
                mapper.Map<AuthorItem>(author), HttpStatusCode.Created));
        }

        // SetAuthorPicture
        private static async Task<IResult> SetAuthorPicture(
            int id, IFormFile imageFile,
            IAuthorRepository authorRepository,
            IMediaManager mediaManager)
        {
            var imageUrl = await mediaManager.SaveFileAsync(
            imageFile.OpenReadStream(),
            imageFile.FileName, imageFile.ContentType);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, "Không lưu được tập tin"));
            }

            await authorRepository.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(ApiResponse.Success(imageUrl));
        }

        // UpdateAuthor
        private static async Task<IResult> UpdateAuthor(
            int id, AuthorEditModel model,
            IValidator<AuthorEditModel> validator,
            IAuthorRepository authorRepository,
           IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, validationResult));
            }

            if (await authorRepository
                   .IsAuthorSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                  $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var author = mapper.Map<Author>(model);
            author.Id = id;

            return await authorRepository.AddOrUpdateAuthorAsync(author)
                ? Results.Ok(ApiResponse.Success("Author is updated", HttpStatusCode.NoContent))
                   : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find author"));
        }

        // DeleteAuthor
        private static async Task<IResult> DeleteAuthor(
            int id, IAuthorRepository authorRepository)
        {
            return await authorRepository.DeleteAuthorByIdAsync(id)
                ? Results.Ok(ApiResponse.Success("Author is deleted", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find author "));
        }
    }
}