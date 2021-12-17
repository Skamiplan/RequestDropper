using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using RequestDropper.CounterKeyBuilders;
using RequestDropper.Handlers;

namespace RequestDropper.Middleware
{
    /// <summary>
    /// The <see cref="RequestDropMiddleware{THandler, TCounter}"/> for RequestDropper.
    /// </summary>
    /// <typeparam name="THandler">The type representing the <see cref="RequestHandler{TCounter}"/></typeparam>
    /// <typeparam name="TCounter">The type of class that will be used to store the counts.</typeparam>
    public abstract class RequestDropMiddleware<THandler, TCounter> where THandler : RequestHandler<TCounter>
    {
        private readonly RequestDelegate _next;

        private readonly THandler _handler;

        private readonly ICounterKeyBuilder _counterKeyBuilder;

        private readonly ILogger<RequestDropperMiddleware<THandler, TCounter>> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestDropMiddleware{THandler, TCounter}"/> class.
        /// </summary>
        /// <param name="next"><see cref="RequestDelegate"/></param>
        /// <param name="handler"><see cref="THandler"/></param>
        /// <param name="counterKeyBuilder"><see cref="ICounterKeyBuilder"/></param>
        /// <param name="logger"><see cref="ILogger"/></param>
        protected RequestDropMiddleware(RequestDelegate next, THandler handler, ICounterKeyBuilder counterKeyBuilder, ILogger<RequestDropperMiddleware<THandler, TCounter>> logger)
        {
            this._next = next;
            this._handler = handler;
            this._counterKeyBuilder = counterKeyBuilder;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (this._handler.GetExcludedPaths().Contains(context.Request.Path))
            {
                this.logger.LogTrace("Ignoring request with path '{path}' due to it being excluded", context.Request.Path);
                await this._next.Invoke(context);
                return;
            }

            var dropCounter = await this._handler.HandleRequestAsync(this._counterKeyBuilder.Build(context));

            if (await this._handler.QuotaExceededAsync(dropCounter))
            {
                await this.ReturnQuotaExceededResponse(context);
                return;
            }
            await this._next.Invoke(context);

            var rule = this._handler.GetRule(context.Response.StatusCode, context.Request.Path);
            if (rule == null)
            {
                this.logger.LogTrace("Rule not found or ignored for statuscode '{statuscode}' on path {path}", context.Response.StatusCode, context.Request.Path);
                return;
            }

            this.logger.LogWarning("Increasing counter for key '{key}' with weight {weight} due to statuscode {statusCode} on path {path}", this._counterKeyBuilder.Build(context), rule.Weight, context.Response.StatusCode, context.Request.Path);

            await this._handler.IncrementCounterAsync(rule, this._counterKeyBuilder.Build(context));
        }

        /// <summary>
        /// Change the <see cref="context"/> to return a quotaexceeded response.
        /// </summary>
        /// <param name="context">The <see cref="context"/> when getting ready to return the response.</param>
        /// <returns><see cref="Task"/></returns>
        protected virtual Task ReturnQuotaExceededResponse(HttpContext context)
        {
            this.logger.LogWarning("Returning Quota Exceeded for key '{key}', threshold of '{threshold}'", this._counterKeyBuilder.Build(context), this._handler.GetLimit());

            if (!string.IsNullOrEmpty(this._handler.GetRedirect()))
            {
                this.logger.LogInformation("Redirecting to '{redirect}' for key '{key}'", this._handler.GetRedirect(), this._counterKeyBuilder.Build(context));
                context.Response.Redirect(this._handler.GetRedirect());
                return Task.CompletedTask;
            }

            this.logger.LogInformation("Returning Too Many Requests response for key '{key}'", this._counterKeyBuilder.Build(context));

            var message = this._handler.GetMessage();
            if (string.IsNullOrEmpty(message))
            {
                message = "Threshold hit";
                this.logger.LogWarning("No response message set, returning default response message '{message}'", message);
            }

            context.Response.Headers["Retry-After"] = this._handler.GetPeriod().TotalSeconds.ToString();

            context.Response.StatusCode = 429;
            context.Response.ContentType = "text/plain";
            return context.Response.WriteAsync(message);
        }
    }
}
