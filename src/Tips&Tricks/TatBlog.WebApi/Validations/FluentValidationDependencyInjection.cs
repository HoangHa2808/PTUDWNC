using FluentValidation;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TatBlog.WebApi.Validations
{
    public static class FluentValidationDependencyInjection
    {
        public static WebApplicationBuilder ConfigurentValidation(
            this WebApplicationBuilder builder)
        {
            // Scan and register all validators in given assembly
            builder.Services.AddValidatorsFromAssembly(
                Assembly.GetExecutingAssembly());

            return builder;
        }
    }
}