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
using TatBlog.WebApi.Models.Posts;
using TatBlog.WebApi.Models.Tags;
using System.Net;

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
                .Produces<ApiResponse<PaginationResult<TagItem>>>();

            // Quản lý thông tin thẻ
            routeGroupBuilder.MapGet("/{id:int}", GetPostByTagId)
                .WithName("GetTagById")
                .Produces<ApiResponse<TagItem>>();

            // GetPostsByTagSlug
            routeGroupBuilder.MapGet(
                "/{slug:regex(^[a-z0-9 -]+$)}/posts", GetPostsByTagSlug)
                .WithName("GetPostsByTagSlug")
                .Produces<ApiResponse<PaginationResult<PostDTO>>>();

            // AddTag
            routeGroupBuilder.MapPost("/", AddTag)
                .WithName("AddNewTag")
                .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
                .Produces(401)
              .Produces<ApiResponse<TagItem>>();

            // UpdateTag
            routeGroupBuilder.MapPut("/{id:int}", UpdateTag)
                .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
               .WithName("UpdateAnTag")
                .Produces(401)
                .Produces<ApiResponse<string>>();

            // DeleteTag
            routeGroupBuilder.MapDelete("/{id:int}", DeleteTag)
                .WithName("DeleteTag")
                 .Produces(401)
                .Produces<ApiResponse<string>>();

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách thẻ
        public static async Task<IResult> GetTags(
            IBlogRepository bolgRepository)
        {
            var tagList = await bolgRepository.GetTagsItemsAsync();

            return Results.Ok(ApiResponse.Success(tagList));
        }

        // GetPostByTagId
        public static async Task<IResult> GetPostByTagId(
           int id,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var tag = await blogRepository.GetCachedTagByIdAsync(id);
            return tag == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy thẻ có mã số {id}"))
                : Results.Ok(ApiResponse.Success(mapper.Map<TagItem>(tag)));
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
            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        // AddTag
        private static async Task<IResult> AddTag(
            TagEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsTagSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                    $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var tag = mapper.Map<Tag>(model);
            await blogRepository.AddOrUpdateTagAsync(tag);

            return Results.Ok(ApiResponse.Success(
                mapper.Map<TagItem>(tag), HttpStatusCode.Created));
        }

        // UpdateTag
        private static async Task<IResult> UpdateTag(
            int id, TagEditModel model,
            IValidator<TagEditModel> validator,
            IBlogRepository blogRepository,
           IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, validationResult));
            }

            if (await blogRepository
                   .IsTagSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                  $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var tag = mapper.Map<Tag>(model);
            tag.Id = id;

            return await blogRepository.AddOrUpdateTagAsync(tag)
               ? Results.Ok(ApiResponse.Success("Tag is updated", HttpStatusCode.NoContent))
                   : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find tag"));
        }

        // DeleteTag
        private static async Task<IResult> DeleteTag(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.DeleteTagByIdAsync(id)
               ? Results.Ok(ApiResponse.Success("Tag is deleted", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find tag"));
        }
    }
}