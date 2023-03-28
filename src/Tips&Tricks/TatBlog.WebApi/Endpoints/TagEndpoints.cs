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
    public static class TagEndpoints
    {
        public static WebApplication MapTagEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/tags");
            // Định nghĩa API endpoint đầu tiên
            routeGroupBuilder.MapGet("/", GetTags)
                .WithName("GetTags")
                .Produces<PaginationResult<TagItem>>();

            // Quản lý thông tin thẻ
            routeGroupBuilder.MapGet("/{id:int}", GetTagDetails)
                .WithName("GetTagById")
                .Produces<TagItem>()
                .Produces(404);

            // GetPostsByTagSlug
            routeGroupBuilder.MapGet(
                "/{slug:regex(^[a-z0-9 -]+$)}/posts", GetPostsByTagSlug)
                .WithName("GetPostsByTagSlug")
                .Produces<PaginationResult<PostDTO>>();

            // AddTag
            routeGroupBuilder.MapPost("/", AddTag)
                .WithName("AddNewTag")
                .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            // UpdateTag
            routeGroupBuilder.MapPut("/{id:int}", UpdateTag)
                .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
               .WithName("UpdateAnTag")
               .Produces(204)
               .Produces(400)
               .Produces(409);

            // DeleteTag
            routeGroupBuilder.MapDelete("/{id:int}", DeleteTag)
                .WithName("DeleteTag")
                .Produces(204)
                .Produces(404);

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách thẻ
        public static async Task<IResult> GetTags(
            [AsParameters] TagFilterModel model,
            IBlogRepository bolgRepository)
        {
            var tagList = await bolgRepository
                .GetPagedTagsAsync(model, model.Name);

            var paginationResult = new PaginationResult<TagItem>(tagList);
            return Results.Ok(paginationResult);
        }

        // GetTagDetails
        public static async Task<IResult> GetTagDetails(
           int id,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var tag = await blogRepository.GetCachedTagByIdAsync(id);
            return tag == null ? Results.NotFound($"Không tìm thấy thẻ có mã số {id}")
                : Results.Ok(mapper.Map<TagItem>(tag));
        }

        // GetPostByTagId
        public static async Task<IResult> GetPostByTagId(
            int id,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                TagId = id,
                PublishedOnly = true
            };

            var postList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                posts => posts.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postList);
            return Results.Ok(paginationResult);
        }

        // GetPostsByTagSlug
        private static async Task<IResult> GetPostsByTagSlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                TagSlug = slug,
                PublishedOnly = true
            };

            var postsList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                postsList => postsList.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postsList);
            return Results.Ok(paginationResult);
        }

        // AddTag
        private static async Task<IResult> AddTag(
            TagEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsTagSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Conflict(
                    $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var tag = mapper.Map<Tag>(model);
            await blogRepository.AddOrUpdateTagAsync(tag);

            return Results.CreatedAtRoute(
                "GetTagById", new { tag.Id },
                mapper.Map<TagItem>(tag));
        }

        // UpdateTag
        private static async Task<IResult> UpdateTag(
            int id, TagEditModel model,
            IBlogRepository blogRepository,
           IMapper mapper)
        {
            if (await blogRepository
                   .IsTagSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Conflict(
                  $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var tag = mapper.Map<Tag>(model);
            tag.Id = id;

            return await blogRepository.AddOrUpdateTagAsync(tag)
                ? Results.NoContent()
                   : Results.NotFound();
        }

        // DeleteTag
        private static async Task<IResult> DeleteTag(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.DeleteTagByIdAsync(id)
                ? Results.NoContent()
                : Results.NotFound($"Could not find tag with id = {id}");
        }
    }
}