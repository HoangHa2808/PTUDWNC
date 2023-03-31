using TatBlog.WebApi.Endpoints;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Mapsters;
using TatBlog.WebApi.Validations;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder
        .ConfigureCors()
        .ConfigureNLog()
        .ConfigureServices()
        .ConfigureSwaggerOpenApi()
        .ConfigureMapster()
        .ConfigurentValidation();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline
    app.SetupRequestPipeline();

    // Configure API endpoints
    app.MapAuthorEndpoints();
    app.MapCategoryEndpoints();
    app.MapTagEndpoints();
    app.MapPostEndpoints();
    app.MapCommentEndpoints();
    app.MapSubscriberEndpoints();
    app.MapDashboardEndpoints();

    app.Run();
}