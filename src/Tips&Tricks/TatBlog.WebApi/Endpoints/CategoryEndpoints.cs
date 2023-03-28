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
    public static class CategoryEndpoints
    {
        public static WebApplication MapCategoryEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/categories");
            // Định nghĩa API endpoint đầu tiên
            routeGroupBuilder.MapGet("/", GetCategoies)
                .WithName("GetCategoies")
                .Produces<PaginationResult<CategoryItem>>();

            // Quản lý thông tin chủ đề
            routeGroupBuilder.MapGet("/{id:int}", GetCategoryDetails)
                .WithName("GetCategoryById")
                .Produces<CategoryItem>()
                .Produces(404);

            // GetPostsByCategoriesSlug
            routeGroupBuilder.MapGet(
                "/{slug:regex(^[a-z0-9 -]+$)}/posts", GetPostsByCategoriesSlug)
                .WithName("GetPostsByCategoriesSlug")
                .Produces<PaginationResult<PostDTO>>();

            // AddCategory
            routeGroupBuilder.MapPost("/", AddCategory)
                .WithName("AddNewCategory")
                .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            // UpdateCategory
            routeGroupBuilder.MapPut("/{id:int}", UpdateCategory)
                .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
               .WithName("UpdateAnCategory")
               .Produces(204)
               .Produces(400)
               .Produces(409);

            // DeleteAuthor
            routeGroupBuilder.MapDelete("/{id:int}", DeleteCategory)
                .WithName("DeleteCategory")
                .Produces(204)
                .Produces(404);

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách chủ đề
        public static async Task<IResult> GetCategoies(
            [AsParameters] CategoryFilterModel model,
            IBlogRepository bolgRepository)
        {
            var categoryList = await bolgRepository
                .GetPagedCategoriesAsync(model, model.Name);

            var paginationResult = new PaginationResult<CategoryItem>(categoryList);
            return Results.Ok(paginationResult);
        }

        // GetCategoryDetails
        public static async Task<IResult> GetCategoryDetails(
           int id,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var category = await blogRepository.GetCachedCategoryByIdAsync(id);
            return category == null ? Results.NotFound($"Không tìm thấy chủ đề có mã số {id}")
                : Results.Ok(mapper.Map<CategoryItem>(category));
        }

        // GetPostByCategoryId
        public static async Task<IResult> GetPostByCategoryId(
            int id,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                CategoryId = id,
                PublishedOnly = true
            };

            var postList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                posts => posts.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postList);
            return Results.Ok(paginationResult);
        }

        // GetPostsByCategoriesSlug
        private static async Task<IResult> GetPostsByCategoriesSlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                CategorySlug = slug,
                PublishedOnly = true
            };

            var postsList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                postsList => postsList.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postsList);
            return Results.Ok(paginationResult);
        }

        // AddCategory
        private static async Task<IResult> AddCategory(
            CategoryEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsCategorySlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Conflict(
                    $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var category = mapper.Map<Category>(model);
            await blogRepository.AddOrUpdateCategoryAsync(category);

            return Results.CreatedAtRoute(
                "GetCategoryById", new { category.Id },
                mapper.Map<CategoryItem>(category));
        }

        // UpdateCategory
        private static async Task<IResult> UpdateCategory(
            int id, CategoryEditModel model,
            IBlogRepository blogRepository,
           IMapper mapper)
        {
            if (await blogRepository
                   .IsCategorySlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Conflict(
                  $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var category = mapper.Map<Category>(model);
            category.Id = id;

            return await blogRepository.AddOrUpdateCategoryAsync(category)
                ? Results.NoContent()
                   : Results.NotFound();
        }

        // DeleteCategory
        private static async Task<IResult> DeleteCategory(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.DeleteCategoryByIdAsync(id)
                ? Results.NoContent()
                : Results.NotFound($"Could not find category with id = {id}");
        }
    }
}