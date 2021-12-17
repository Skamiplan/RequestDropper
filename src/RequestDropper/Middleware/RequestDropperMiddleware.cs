using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using RequestDropper.CounterKeyBuilders;
using RequestDropper.Handlers;

namespace RequestDropper.Middleware
{
    public class RequestDropperMiddleware<THandler, TCounter> : RequestDropMiddleware<THandler, TCounter> where THandler : RequestHandler<TCounter>
    {
        public RequestDropperMiddleware(RequestDelegate next, THandler handler, ICounterKeyBuilder counterKeyBuilder, ILogger<RequestDropperMiddleware<THandler, TCounter>> logger)
            : base(next, handler, counterKeyBuilder, logger)
        {
        }
    }
}
