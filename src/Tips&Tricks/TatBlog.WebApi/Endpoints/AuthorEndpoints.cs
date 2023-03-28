using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Authors;
using TatBlog.Services.Media;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints
{
    public static class AuthorEndpoints
    {
        public static WebApplication MapAuthorEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/authors");
            // Định nghĩa API endpoint đầu tiên
            routeGroupBuilder.MapGet("/", GetAuthors)
                .WithName("GetAuthors")
                .Produces<PaginationResult<AuthorItem>>();

            // Quản lý thông tin tác giả
            routeGroupBuilder.MapGet("/{id:int}", GetAuthorDetails)
                .WithName("GetAuthorById")
                .Produces<AuthorItem>()
                .Produces(404);

            // GetPostsByAuthorsSlug
            routeGroupBuilder.MapGet(
                "/{slug:regex(^[a-z0-9 -]+$)}/posts", GetPostsByAuthorsSlug)
                .WithName("GetPostsByAuthorsSlug")
                .Produces<PaginationResult<PostDTO>>();

            // ListAuthorBest
            routeGroupBuilder.MapGet("/best/{limit:int}", ListAuthorBest)
              .WithName("ListAuthorBest")
              .Produces<AuthorItem>()
              .Produces(404);

            // AddAuthor
            routeGroupBuilder.MapPost("/", AddAuthor)
                .WithName("AddNewAuthor")
                .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            // SetAuthorPicture
            routeGroupBuilder.MapPost("/{id:int}/avatar", SetAuthorPicture)
                .WithName("SetAuthorPicture")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<string>()
                .Produces(404);

            // UpdateAuthor
            routeGroupBuilder.MapPut("/{id:int}", UpdateAuthor)
                .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
               .WithName("UpdateAnAuthor")
               .Produces(204)
               .Produces(400)
               .Produces(409);

            // DeleteAuthor
            routeGroupBuilder.MapDelete("/{id:int}", DeleteAuthor)
                .WithName("DeleteAuthor")
                .Produces(204)
                .Produces(404);

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách tác giả
        public static async Task<IResult> GetAuthors(
            [AsParameters] AuthorFilterModel model,
            IAuthorRepository authorRepository)
        {
            var authorList = await authorRepository
                .GetPagedAuthorsAsync(model, model.Name);

            var paginationResult = new PaginationResult<AuthorItem>(authorList);
            return Results.Ok(paginationResult);
        }

        // GetAuthorDetails
        public static async Task<IResult> GetAuthorDetails(
           int id,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            var author = await authorRepository.GetCachedAuthorByIdAsync(id);
            return author == null ? Results.NotFound($"Không tìm thấy tác giả có mã số {id}")
                : Results.Ok(mapper.Map<AuthorItem>(author));
        }

        // GetPostByAuthorId
        public static async Task<IResult> GetPostByAuthorId(
            int id,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                AuthorId = id,
                PublishedOnly = true
            };

            var postList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                posts => posts.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postList);
            return Results.Ok(paginationResult);
        }

        // GetPostsByAuthorsSlug
        private static async Task<IResult> GetPostsByAuthorsSlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                AuthorSlug = slug,
                PublishedOnly = true
            };

            var postsList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                postsList => postsList.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postsList);
            return Results.Ok(paginationResult);
        }

        // ListAuthorBest
        public static async Task<IResult> ListAuthorBest(
           int N,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            var author = await authorRepository.ListAuthorAsync(N);
            return author == null ? Results.NotFound($"Không tìm thấy danh sách {N} tác giả có nhiều bài viết đóng góp cho blog nhất")
                : Results.Ok(mapper.Map<AuthorItem>(author));
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
                return Results.Conflict(
                    $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var author = mapper.Map<Author>(model);
            await authorRepository.AddOrUpdateAuthorAsync(author);

            return Results.CreatedAtRoute(
                "GetAuthorById", new { author.Id },
                mapper.Map<AuthorItem>(author));
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
                return Results.BadRequest("Không lưu được tập tin");
            }

            await authorRepository.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(imageUrl);
        }

        // UpdateAuthor
        private static async Task<IResult> UpdateAuthor(
            int id, AuthorEditModel model,
            //IValidator<AuthorEditModel> validator,
            IAuthorRepository authorRepository,
           IMapper mapper)
        {
            //var validationResult = await validator.ValidateAsync(model);

            //if (!validationResult.IsValid)
            //{
            //    return Results.BadRequest(
            //        validationResult.Errors.ToResponse());
            //}

            if (await authorRepository
                   .IsAuthorSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Conflict(
                  $"Slug '{model.UrlSlug}' da được sử dụng");
            }

            var author = mapper.Map<Author>(model);
            author.Id = id;

            return await authorRepository.AddOrUpdateAuthorAsync(author)
                ? Results.NoContent()
                   : Results.NotFound();
        }

        // DeleteAuthor
        private static async Task<IResult> DeleteAuthor(
            int id, IAuthorRepository authorRepository)
        {
            return await authorRepository.DeleteAuthorByIdAsync(id)
                ? Results.NoContent()
                : Results.NotFound($"Could not find author with id = {id}");
        }
    }
}