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
    public static class PostEndpoints
    {
        public static WebApplication MapPostEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/posts");
            // Định nghĩa API endpoint đầu tiên
            //routeGroupBuilder.MapGet("/", GetPosts)
            //    .WithName("GetPosts")
            //    .Produces<PaginationResult<PostQuery>>();

            // ListPostFeatured
            routeGroupBuilder.MapGet("/featured/{limit:int}", ListPostFeatured)
              .WithName("ListPostFeatured")
              .Produces<PostQuery>()
              .Produces(404);

            // ListPostRandom
            routeGroupBuilder.MapGet("/random/{limit:int}", ListPostRandom)
              .WithName("ListPostRandom")
              .Produces<PostQuery>()
              .Produces(404);

            // ListPostArchives
            routeGroupBuilder.MapGet("/archives/{limit:int}", ListPostArchives)
              .WithName("ListPostArchives")
              .Produces<PostQuery>()
              .Produces(404);

            // Quản lý thông tin bài viết
            routeGroupBuilder.MapGet("/{id:int}", GetPostDetails)
                .WithName("GetPostById")
                .Produces<PostQuery>()
                .Produces(404);

            // Lấy danh sách bình luận của bài viết
            //routeGroupBuilder.MapGet("/{id:int}/comments", GetPostComment)
            //    .WithName("GetPostComment")
            //    .Produces<PostQuery>()
            //    .Produces(404);

            // GetPostsByPostsSlug
            routeGroupBuilder.MapGet(
                "/byslug/{slug:regex(^[a-z0-9 -]+$)}", GetPostsBySlug)
                .WithName("GetPostsByPostsSlug")
                .Produces<PaginationResult<PostDTO>>();

            // AddPost
            routeGroupBuilder.MapPost("/", AddPost)
                .WithName("AddNewPost")
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            // SetPostPicture
            routeGroupBuilder.MapPost("/{id:int}/picture", SetPostPicture)
                .WithName("SetPostPicture")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<string>()
                .Produces(404);

            // UpdatePost
            routeGroupBuilder.MapPut("/{id:int}", UpdatePost)
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
               .WithName("UpdateAnPost")
               .Produces(204)
               .Produces(400)
               .Produces(409);

            // DeletePost
            routeGroupBuilder.MapDelete("/{id:int}", DeletePost)
                .WithName("DeletePost")
                .Produces(204)
                .Produces(404);

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách tác giả
        //public static async Task<IResult> GetPosts(
        //    [AsParameters] PostFilterModel model,
        //    IBlogRepository bolgRepository)
        //{
        //    var postList = await bolgRepository
        //        .GetPagedPostAsync(model, model.Title);

        //    var paginationResult = new PaginationResult<PostQuery>(postList);
        //    return Results.Ok(paginationResult);
        //}

        // ListPostFeatured
        public static async Task<IResult> ListPostFeatured(
           int N,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.GetPopularArticlesAsync(N);
            return post == null ? Results.NotFound($"Không tìm thấy danh sách {N} bài viết được nhiều người đọc nhất")
                : Results.Ok(mapper.Map<PostQuery>(post));
        }

        // ListPostRandom
        public static async Task<IResult> ListPostRandom(
           int N,
           IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.GetRandomPostsAsync(N);
            return post == null ? Results.NotFound($"Không tìm thấy ngẫu nhiên một danh sách {N} bài viết")
                : Results.Ok(mapper.Map<PostQuery>(post));
        }

        // ListPostArchives
        public static async Task<IResult> ListPostArchives(
           int N,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.CountPostsMonthAsync(N);
            return post == null ? Results.NotFound($"Không tìm thấy danh sách thống kê số lượng bài viết trong {N} tháng gần nhất")
                : Results.Ok(mapper.Map<PostQuery>(post));
        }

        // GetPostDetails
        public static async Task<IResult> GetPostDetails(
           int id,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var category = await blogRepository.GetCachedPostByIdAsync(id);
            return category == null ? Results.NotFound($"Không tìm thấy chủ đề có mã số {id}")
                : Results.Ok(mapper.Map<CategoryItem>(category));
        }

        // GetPostComment
        //public static async Task<IResult> GetPostComment(
        //   int id,
        //    IBlogRepository blogRepository,
        //    IMapper mapper)
        //{
        //    var category = await blogRepository.GetCachedCategoryByIdAsync(id);
        //    return category == null ? Results.NotFound($"Không tìm thấy danh sách bình luận của bài viết")
        //        : Results.Ok(mapper.Map<Comment>(category));
        //}

        // GetPostById
        public static async Task<IResult> GetPostById(
            int id,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                PostId = id,
                PublishedOnly = true
            };

            var postList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                posts => posts.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postList);
            return Results.Ok(paginationResult);
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
            return Results.Ok(paginationResult);
        }

        // AddPost
        private static async Task<IResult> AddPost(
            PostEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsPostSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Conflict(
                    $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var post = mapper.Map<Post>(model);
            await blogRepository.AddOrUpdatePostsAsync(post);

            return Results.CreatedAtRoute(
                "GetPostById", new { post.Id },
                mapper.Map<PostQuery>(post));
        }

        // SetAuthorPicture
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
                return Results.BadRequest("Không lưu được tập tin");
            }

            await blogRepository.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(imageUrl);
        }

        // UpdatePost
        private static async Task<IResult> UpdatePost(
            int id, PostEditModel model,
            IBlogRepository blogRepository,
           IMapper mapper)
        {
            if (await blogRepository
                   .IsPostSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Conflict(
                  $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var post = mapper.Map<Post>(model);
            post.Id = id;

            return await blogRepository.AddOrUpdatePostsAsync(post)
                ? Results.NoContent()
                   : Results.NotFound();
        }

        // DeletePost
        private static async Task<IResult> DeletePost(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.DeletePostsByIdAsync(id)
                ? Results.NoContent()
                : Results.NotFound($"Could not find post with id = {id}");
        }
    }
}