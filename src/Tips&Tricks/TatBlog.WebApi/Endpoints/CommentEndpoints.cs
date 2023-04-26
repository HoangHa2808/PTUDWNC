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
using TatBlog.WebApi.Models.Comments;
using TatBlog.WebApi.Models;
using System.Net;

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
                .Produces<ApiResponse<PaginationResult<Comment>>>();

            // Quản lý thông tin bình luận
            routeGroupBuilder.MapPost("/{id:int}", ChangesCommentStatus)
                .WithName("ChangesCommentStatus")
                 .Produces(401)
                .Produces<ApiResponse<string>>();

            // DeleteComment
            routeGroupBuilder.MapDelete("/{id:int}", DeleteComment)
                .WithName("DeleteComment")
                .Produces(401)
                .Produces<ApiResponse<string>>();

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách bình luận
        public static async Task<IResult> GetComments(
            
            IBlogRepository blogRepository)
        {
            var commentList = await blogRepository
                .GetCommentsAsync();

            return Results.Ok(ApiResponse.Success(commentList));
            
        }

        // ChangesCommentStatus
        private static async Task<IResult> ChangesCommentStatus(
            int id,
            IBlogRepository blogRepository)
        {
            return await blogRepository.ChangeCommentByIdAsync(id)
                  ? Results.Ok(ApiResponse.Success("Comment is changed", HttpStatusCode.NoContent))
                 : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find comment"));
        }

        // DeleteComment
        private static async Task<IResult> DeleteComment(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.DeleteCommentByIdAsync(id)
                 ? Results.Ok(ApiResponse.Success("Comment is deleted", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find comment"));
        }
    }
}