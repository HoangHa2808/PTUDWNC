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
using System.Collections.Generic;
using TatBlog.WebApi.Models.Posts;
using TatBlog.Core.Contracts;
using System.Net;

namespace TatBlog.WebApi.Endpoints
{
    public static class PostEndpoints
    {
        public static WebApplication MapPostEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/posts");
            // Định nghĩa API endpoint đầu tiên
            // Lấy danh sách bài viết.
            // Hỗ trợ tìm theo từ khóa, chuyên mục, tác giả, ngày đăng, … và phân trang kết quả
            routeGroupBuilder.MapGet("/", GetPosts)
                .WithName("GetPosts")
                .Produces<ApiResponse<PaginationResult<PostDTO>>>();

            // ListPostFeatured
            // Lấy danh sách N (limit) bài viết được nhiều người đọc nhất
            routeGroupBuilder.MapGet("/featured/{limit:int}", GetListPostFeatured)
              .WithName("GetPostFeatured")
              .Produces<ApiResponse<IList<PostDTO>>>();

            // ListPostRandom
            // Lấy ngẫu nhiên một danh sách N(limit) bài viết
            routeGroupBuilder.MapGet("/random/{limit:int}", GetListPostRandom)
              .WithName("GetPostRandom")
              .Produces<ApiResponse<IList<PostDTO>>>();

            // ListPostArchives
            // Lấy danh sách thống kê số lượng bài viết trong N(limit) tháng gần nhất
            routeGroupBuilder.MapGet("/archives/{limit:int}", GetListPostArchives)
              .WithName("GetPostArchives")
              .Produces<ApiResponse<IList<PostItem>>>();

            // Quản lý thông tin bài viết
            // Lấy thông tin chi tiết của bài viết có mã số(id) cho trước
            routeGroupBuilder.MapGet("/{id:int}", GetPostById)
                .WithName("GetPostById")
                .Produces<ApiResponse<PostDetail>>();

            // Lấy danh sách bình luận của bài viết
            routeGroupBuilder.MapGet("/{id:int}/comments", GetPostComment)
                .WithName("GetPostComment")
                .Produces<ApiResponse<IList<PostDTO>>>();

            // GetPostsByPostsSlug
            // Lấy thông tin chi tiết bài viết có tên định danh(slug) cho trước
            routeGroupBuilder.MapGet(
                "/byslug/{slug:regex(^[a-z0-9 -]+$)}", GetPostsBySlug)
                .WithName("GetPostsBySlug")
                .Produces<ApiResponse<PostDetail>>();

            // AddPost
            routeGroupBuilder.MapPost("/", AddPost)
                .WithName("AddNewPost")
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
                .Produces(401)
                .Produces<ApiResponse<PostDetail>>();

            // SetPostPicture
            // Tải lên hình ảnh đại diện cho bài viết
            routeGroupBuilder.MapPost("/{id:int}/picture", SetPostPicture)
                .WithName("SetPostPicture")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<ApiResponse<string>>();

            // UpdatePost
            routeGroupBuilder.MapPut("/{id:int}", UpdatePost)
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
               .WithName("UpdateAnPost")
               .Produces(401)
                .Produces<ApiResponse<string>>();

            // DeletePost
            routeGroupBuilder.MapDelete("/{id:int}", DeletePost)
                .WithName("DeletePost")
                .Produces(401)
                .Produces<ApiResponse<string>>();

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách bài viết
        public static async Task<IResult> GetPosts(
            [AsParameters] PostFilterModel model,
            IBlogRepository bolgRepository,
            IMapper mapper)
        {
            var postQuery = mapper.Map<PostQuery>(model);
            var postList = await bolgRepository
                .GetPagedPostsAsync(postQuery, model, posts => posts.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        // ListPostFeatured
        public static async Task<IResult> GetListPostFeatured(
           int limit,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.GetPopularArticlesAsync(limit);
            return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết được nhiều người đọc nhất"))
                   : Results.Ok(ApiResponse.Success(mapper.Map<IList<PostDTO>>(post)));
        }

        // ListPostRandom
        public static async Task<IResult> GetListPostRandom(
           int limit,
           IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.GetRandomPostsAsync(limit);
            return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy ngẫu nhiên bài viết nào"))
                : Results.Ok(ApiResponse.Success(mapper.Map<IList<PostDTO>>(post)));
        }

        // ListPostArchives
        public static async Task<IResult> GetListPostArchives(
           int limit,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.CountPostsMonthAsync(limit);
            return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết trong {limit} tháng gần nhất"))
                : Results.Ok(ApiResponse.Success(mapper.Map<IList<PostItem>>(post)));
        }

        // GetPostComment
        public static async Task<IResult> GetPostComment(
           int id,
            IBlogRepository blogRepository)
        {
            var post = await blogRepository.GetPostCommentsAsync(id);
            return Results.Ok(ApiResponse.Success(post));
        }

        // GetPostById
        public static async Task<IResult> GetPostById(
            int id,
            IMapper mapper,
            IBlogRepository blogRepository)
        {
            var posts = await blogRepository.GetPostByIdAsync(id);

            return posts != null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết có mã số {id}"))
               : Results.Ok(ApiResponse.Success(mapper.Map<PostDetail>(posts)));
        }

        // GetPostsBySlug
        private static async Task<IResult> GetPostsBySlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                PostSlug = slug,
                PublishedOnly = true
            };

            var postsList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                postsList => postsList.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postsList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        // AddPost
        private static async Task<IResult> AddPost(
            PostEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsPostSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                    $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var post = mapper.Map<Post>(model);
            post.PostedDate = DateTime.Now;
            await blogRepository.AddOrUpdatePostsAsync(post);

            return Results.Ok(ApiResponse.Success(
                mapper.Map<PostQuery>(post), HttpStatusCode.Created));
        }

        // SetPostPicture
        private static async Task<IResult> SetPostPicture(
            int id, IFormFile imageFile,
            IBlogRepository blogRepository,
            IMediaManager mediaManager)
        {
            var imageUrl = await mediaManager.SaveFileAsync(
            imageFile.OpenReadStream(),
            imageFile.FileName, imageFile.ContentType);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, "Không lưu được tập tin"));
            }

            await blogRepository.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(ApiResponse.Success(imageUrl));
        }

        // UpdatePost
        private static async Task<IResult> UpdatePost(
            int id, PostEditModel model,
            IValidator<PostEditModel> validator,
            IBlogRepository blogRepository,
           IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, validationResult));
            }

            if (await blogRepository
                   .IsPostSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                 $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var post = mapper.Map<Post>(model);
            post.Id = id;
            post.Category = null;
            post.Author = null;

            return await blogRepository.AddOrUpdatePostsAsync(post)
                  ? Results.Ok(ApiResponse.Success("Post is updated", HttpStatusCode.NoContent))
                   : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find post"));
        }

        // DeletePost
        private static async Task<IResult> DeletePost(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.DeletePostsByIdAsync(id)
                 ? Results.Ok(ApiResponse.Success("Post is deleted", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find post"));
        }
    }
}