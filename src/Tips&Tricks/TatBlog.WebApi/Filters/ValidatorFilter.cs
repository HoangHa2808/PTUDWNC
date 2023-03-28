using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TatBlog.Core.DTO;
using TatBlog.WebApi.Endpoints;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Filters
{
    public class ValidatorFilter<T> : IEndpointFilter where T : class
    {
        private readonly IValidator<T> _validator;
        private readonly ILogger<T> _logger;

        public ValidatorFilter(IValidator<T> validator, ILogger<T> logger)
        {
            _validator = validator;
            _logger = logger;
        }

        public async ValueTask<object> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)

        {
            var model = context.Arguments
                .SingleOrDefault(x => x?.GetType() == typeof(T)) as T;

            if (model == null)
            {
                return Results.BadRequest(
                    new ValidationFailureResponse(new[]
                    {
                        "Could not create model object"
                    }));
            }

            var validationResult = await _validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(
                    validationResult.Errors.ToResponse());
            }

            return await next(context);
        }

        public async Task Invoke(HttpContext context,
            RequestDelegate next)
        {
            _logger.LogInformation(
                "{Time:yyyy-MM-dd HH:mm:ss} - IP: {IpAddress} - Path: {Url}",
                DateTime.Now,
                context.Connection.RemoteIpAddress?.ToString(),
            context.Request.Path);

            await next(context);
        }
    }
}