using Microsoft.AspNetCore.Builder;

using RequestDropper.Handlers;
using RequestDropper.Middleware;
using RequestDropper.Models;

namespace RequestDropper.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds middleware for dropping unwanted requests.
        /// </summary>
        /// <typeparam name="THandler">The type representing the <see cref="RequestHandler{TCounter}"/></typeparam>
        /// <typeparam name="TCounter">The type representing the <see cref="TCounter"/></typeparam>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> for RequestDropper.</returns>
        public static IApplicationBuilder UseRequestDropperMiddleware<THandler, TCounter>(
            this IApplicationBuilder builder)
            where THandler : RequestHandler<TCounter>
        {
            return builder.UseMiddleware<RequestDropperMiddleware<THandler, TCounter>>();
        }

        /// <summary>
        /// Adds default middleware for dropping unwanted requests.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> for RequestDropper.</returns>
        public static IApplicationBuilder UseRequestDropperMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestDropperMiddleware<NativeRequestHandler, DropCounter?>>();
        }
    }
}
