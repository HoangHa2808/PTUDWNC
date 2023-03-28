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
    public static class CommentEndpoints
    {
        public static WebApplication MapCommentEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/comments");
            // Định nghĩa API endpoint đầu tiên
            routeGroupBuilder.MapGet("/", GetComments)
                .WithName("GetComments")
                .Produces<PaginationResult<Comment>>();

            // Quản lý thông tin bình luận
            routeGroupBuilder.MapGet("/{id:int}", GetCommentDetails)
                .WithName("GetCommentById")
                .Produces<Comment>()
                .Produces(404);

            // AddComment
            routeGroupBuilder.MapPost("/", AddComment)
                .WithName("AddNewComment")
                .AddEndpointFilter<ValidatorFilter<CommentEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            // UpdateComment
            routeGroupBuilder.MapPut("/{id:int}", UpdateComment)
                .AddEndpointFilter<ValidatorFilter<CommentEditModel>>()
               .WithName("UpdateAnComment")
               .Produces(204)
               .Produces(400)
               .Produces(409);

            // DeleteComment
            routeGroupBuilder.MapDelete("/{id:int}", DeleteComment)
                .WithName("DeleteComment")
                .Produces(204)
                .Produces(404);

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách tác giả
        public static async Task<IResult> GetComments(
            [AsParameters] CommentFilterModel model,
            IBlogRepository blogRepository)
        {
            var commentList = await blogRepository
                .GetPagedCommentsAsync(model, model.Name);

            var paginationResult = new PaginationResult<Comment>(commentList);
            return Results.Ok(paginationResult);
        }

        // GetAuthorDetails
        public static async Task<IResult> GetCommentDetails(
           int id,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var comment = await blogRepository.GetCachedCommentByIdAsync(id);
            return comment == null ? Results.NotFound($"Không tìm thấy tác giả có mã số {id}")
                : Results.Ok(mapper.Map<Comment>(comment));
        }

        //// GetPostByAuthorId
        //public static async Task<IResult> GetPostByAuthorId(
        //    int id,
        //    [AsParameters] PagingModel pagingModel,
        //    IBlogRepository blogRepository)
        //{
        //    var postQuery = new PostQuery()
        //    {
        //        AuthorId = id,
        //        PublishedOnly = true
        //    };

        //    var postList = await blogRepository.GetPagedPostsAsync(
        //        postQuery, pagingModel,
        //        posts => posts.ProjectToType<PostDTO>());

        //    var paginationResult = new PaginationResult<PostDTO>(postList);
        //    return Results.Ok(paginationResult);
        //}

        // AddComment
        private static async Task<IResult> AddComment(
            CommentEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsCommentExistedAsync(0, model.Description))
            {
                return Results.Conflict(
                    $"Slug '{model.Description}' đã được sử dụng");
            }

            var comment = mapper.Map<Comment>(model);
            await blogRepository.AddOrUpdateCommentAsync(comment);

            return Results.CreatedAtRoute(
                "GetCommentById", new { comment.Id },
                mapper.Map<Comment>(comment));
        }

        // UpdateComment
        private static async Task<IResult> UpdateComment(
            int id, CommentEditModel model,
            IBlogRepository blogRepository,
           IMapper mapper)
        {
            if (await blogRepository
                   .IsCommentExistedAsync(id, model.Description))
            {
                return Results.Conflict(
                  $"Slug '{model.Description}' da được sử dụng");
            }

            var comment = mapper.Map<Comment>(model);
            comment.Id = id;

            return await blogRepository.AddOrUpdateCommentAsync(comment)
                ? Results.NoContent()
                   : Results.NotFound();
        }

        // DeleteComment
        private static async Task<IResult> DeleteComment(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.DeleteCommentByIdAsync(id)
                ? Results.NoContent()
                : Results.NotFound($"Could not find comment with id = {id}");
        }
    }
}