using FluentValidation;
using MapsterMapper;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.Contracts;
using TatBlog.Core.Entities;
using TatBlog.Services.Subscribers;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;
using TatBlog.WebApi.Models.Subscribers;

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
                .Produces<ApiResponse<IPagedList<Subscriber>>>();

            // BlockSubscriber
            routeGroupBuilder.MapPut("/{id:int}", BlockSubscriber)
               .WithName("BlockSubscriber")
               .Produces<ApiResponse<string>>();

            // DeleteSubscriber
            routeGroupBuilder.MapDelete("/{id:int}", DeleteSubscriber)
                .WithName("DeleteSubscriber")
                .Produces(401)
                .Produces<ApiResponse<string>>();

            return app;
        }

        // Xử lý yêu cầu tìm và lấy danh sách đăng ký
        public static async Task<IResult> GetSubscribers(
            [AsParameters] SubscriberFilterModel model,
            ISubscriberRepository subscriberRepository)
        {
            var subscriberList = await subscriberRepository
                .GetPagedSubscribersAsync(model, model.Email);

            var paginationResult = new PaginationResult<Subscriber>(subscriberList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        public static async Task<IResult> BlockSubscriber(
           int id,
            ISubscriberRepository subscriberRepository,
            SubscriberEditModel model)
        {
            return await subscriberRepository.BlockSubscriberAsync(id, model.Resons, model.Notes)
             ? Results.Ok(ApiResponse.Success("Successfully blocked", HttpStatusCode.NoContent))
             : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find subscribers"));
        }

        // DeleteSubscriber
        private static async Task<IResult> DeleteSubscriber(
            int id, ISubscriberRepository subscriberRepository)
        {
            return await subscriberRepository.DeleteSubscriberAsync(id)
                ? Results.Ok(ApiResponse.Success("Delete successfully", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find subscribers "));
        }
    }
}