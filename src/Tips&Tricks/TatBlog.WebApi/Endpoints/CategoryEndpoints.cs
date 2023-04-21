using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;
using TatBlog.WebApi.Models.Categories;
using TatBlog.WebApi.Models.Posts;
using System.Net;

namespace TatBlog.WebApi.Endpoints
{
    public static class CategoryEndpoints
    {
        public static WebApplication MapCategoryEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/categories");
            // Định nghĩa API endpoint đầu tiên
            // Lấy danh sách chuyên mục. Hỗ trợ tìm theo tên và phân trang kết quả
            routeGroupBuilder.MapGet("/", GetCategoies)
                .WithName("GetCategoies")
                .Produces<ApiResponse<PaginationResult<CategoryItem>>>();

            // Quản lý thông tin chủ đề
            // Lấy thông tin chi tiết của chuyên mục có mã số(id) cho trước
            routeGroupBuilder.MapGet("/{id:int}", GetPostByCategoryId)
                .WithName("GetCategoryById")
                .Produces<ApiResponse<CategoryItem>>()
                .Produces(404);

            // GetPostsByCategoriesSlug
            // Lấy danh sách những bài viết được đăng trong chuyên mục có tên định danh(slug) cho trước.
            // Hỗ trợ việc phân trang danh sách bài viết
            routeGroupBuilder.MapGet(
                "/{slug:regex(^[a-z0-9 -]+$)}/posts", GetPostsByCategoriesSlug)
                .WithName("GetPostsByCategoriesSlug")
                .Produces<ApiResponse<PaginationResult<PostDTO>>>();

            // AddCategory
            routeGroupBuilder.MapPost("/", AddCategory)
                .WithName("AddNewCategory")
                .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
                .Produces(401)
                .Produces<ApiResponse<CategoryItem>>();

            // UpdateCategory
            routeGroupBuilder.MapPut("/{id:int}", UpdateCategory)
                .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
               .WithName("UpdateAnCategory")
               .Produces(401)
               .Produces<ApiResponse<string>>();

            // DeleteAuthor
            routeGroupBuilder.MapDelete("/{id:int}", DeleteCategory)
                .WithName("DeleteCategory")
                .Produces(401)
                .Produces<ApiResponse<string>>();

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách chủ đề
        public static async Task<IResult> GetCategoies(
           IBlogRepository blogRepository)
         {
                var categoryList = await blogRepository
                .GetCategoriesAsync();
                return Results.Ok(ApiResponse.Success(categoryList));
        }

        // GetPostByCategoryId
        public static async Task<IResult> GetPostByCategoryId(
           int id,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var category = await blogRepository.GetCachedCategoryByIdAsync(id);
            return category == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy chủ đề có mã số {id}"))
                : Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category)));
        }

        // GetCategoryDetail
        //public static async Task<IResult> GetCategoryDetail(
        //    int id,
        //    [AsParameters] PagingModel pagingModel,
        //    IBlogRepository blogRepository)
        //{
        //    var postQuery = new PostQuery()
        //    {
        //        CategoryId = id,
        //        PublishedOnly = true
        //    };

        //    var postList = await blogRepository.GetPagedPostsAsync(
        //        postQuery, pagingModel,
        //        posts => posts.ProjectToType<PostDTO>());

        //    var paginationResult = new PaginationResult<PostDTO>(postList);
        //    return Results.Ok(paginationResult);
        //}

        // GetPostsByCategoriesSlug
        private static async Task<IResult> GetPostsByCategoriesSlug(
             string slug,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var category = await blogRepository.FindCategoryByUrlAsync(slug);

            return category == null ? Results.Ok(ApiResponse.Fail(
                HttpStatusCode.NotFound, $"Không tìm thấy danh mục có tên định danh {slug}"))
               : Results.Ok(ApiResponse.Success(mapper.Map<PostDetail>(category)));
        }

        // AddCategory
        private static async Task<IResult> AddCategory(
            CategoryEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsCategorySlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                    $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var category = mapper.Map<Category>(model);
            await blogRepository.AddOrUpdateCategoryAsync(category);

            return Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category), HttpStatusCode.Created));
        }

        // UpdateCategory
        private static async Task<IResult> UpdateCategory(
            int id, CategoryEditModel model,
            IValidator<CategoryEditModel> validator,
            IBlogRepository blogRepository,
           IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, validationResult));
            }

            if (await blogRepository
                   .IsCategorySlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                  $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var category = mapper.Map<Category>(model);
            category.Id = id;

            return await blogRepository.AddOrUpdateCategoryAsync(category)
               ? Results.Ok(ApiResponse.Success("Category is updated", HttpStatusCode.NoContent))
                   : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find category"));
        }

        // DeleteCategory
        private static async Task<IResult> DeleteCategory(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.DeleteCategoryByIdAsync(id)
                ? Results.Ok(ApiResponse.Success("Category is deleted", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find category"));
        }
    }
}