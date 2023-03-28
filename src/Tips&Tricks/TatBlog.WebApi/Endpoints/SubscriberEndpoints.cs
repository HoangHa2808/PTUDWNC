using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Media;
using TatBlog.Services.Subscribers;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints
{
    public static class SubscriberEndpoints
    {
        public static WebApplication MapSubscriberEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/subscribers");
            // Định nghĩa API endpoint đầu tiên
            routeGroupBuilder.MapGet("/", GetSubscribers)
                .WithName("GetSubscribers")
                .Produces<PaginationResult<Subscriber>>();

            // Quản lý thông tin bình luận
            routeGroupBuilder.MapGet("/{id:int}", GetSubscriberDetails)
                .WithName("GetSubscriberById")
                .Produces<Subscriber>()
                .Produces(404);

            // AddSubscriber
            routeGroupBuilder.MapPost("/", AddSubscriber)
                .WithName("AddNewSubscriber")
                .AddEndpointFilter<ValidatorFilter<SubscriberEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            // UpdateSubscriber
            routeGroupBuilder.MapPut("/{id:int}", UpdateSubscriber)
                .AddEndpointFilter<ValidatorFilter<SubscriberEditModel>>()
               .WithName("UpdateSubscriber")
               .Produces(204)
               .Produces(400)
               .Produces(409);

            // DeleteSubscriber
            routeGroupBuilder.MapDelete("/{id:int}", DeleteSubscriber)
                .WithName("DeleteSubscriber")
                .Produces(204)
                .Produces(404);

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách tác giả
        public static async Task<IResult> GetSubscribers(
            [AsParameters] SubscriberFilterModel model,
            ISubscriberRepository subscriberRepository)
        {
            var subscriberList = await subscriberRepository
                .GetPagedSubscribersAsync(model, model.Email);

            var paginationResult = new PaginationResult<Subscriber>(subscriberList);
            return Results.Ok(paginationResult);
        }

        // GetSubscriberDetails
        public static async Task<IResult> GetSubscriberDetails(
           int id,
            ISubscriberRepository subscriberRepository,
            IMapper mapper)
        {
            var subscriber = await subscriberRepository.GetCachedSubscriberByIdAsync(id);
            return subscriber == null ? Results.NotFound($"Không tìm thấy đăng ký có mã số {id}")
                : Results.Ok(mapper.Map<Subscriber>(subscriber));
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

        // AddSubscriber
        private static async Task<IResult> AddSubscriber(
            SubscriberEditModel model,
            ISubscriberRepository subscriberRepository,
            IMapper mapper)
        {
            if (await subscriberRepository.IsSubscriberExistedEmail(0, model.Email))
            {
                return Results.Conflict(
                    $"Email '{model.Email}' đã được sử dụng");
            }

            var subscriber = mapper.Map<Subscriber>(model);
            await subscriberRepository.AddOrUpdateSubscriberAsync(subscriber);

            return Results.CreatedAtRoute(
                "GetSubscriberById", new { subscriber.Id },
                mapper.Map<Subscriber>(subscriber));
        }

        // UpdateSubscriber
        private static async Task<IResult> UpdateSubscriber(
            int id, SubscriberEditModel model,
             ISubscriberRepository subscriberRepository,
           IMapper mapper)
        {
            if (await subscriberRepository
                   .IsSubscriberExistedEmail(id, model.Email))
            {
                return Results.Conflict(
                  $"Email '{model.Email}' đã được sử dụng");
            }

            var subscriber = mapper.Map<Subscriber>(model);
            subscriber.Id = id;

            return await subscriberRepository.AddOrUpdateSubscriberAsync(subscriber)
                ? Results.NoContent()
                   : Results.NotFound();
        }

        // DeleteSubscriber
        private static async Task<IResult> DeleteSubscriber(
            int id, ISubscriberRepository subscriberRepository)
        {
            return await subscriberRepository.DeleteSubscriberAsync(id)
                ? Results.NoContent()
                : Results.NotFound($"Could not find subscriber with id = {id}");
        }
    }
}