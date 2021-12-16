using Microsoft.AspNetCore.Builder;

using RequestDropper.Extensions;
using RequestDropper.Mongo.Handlers;
using RequestDropper.Mongo.Models;

namespace RequestDropper.Mongo.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestDropperMongoMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseRequestDropperMiddleware<MongoRequestHandler, MongoDropCounter?>();
        }
    }
}
