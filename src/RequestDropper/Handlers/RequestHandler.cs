using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using RequestDropper.Models;

namespace RequestDropper.Handlers
{
    /// <summary>
    /// The <see cref="RequestHandler{TCounter}"/> for RequestDropper.
    /// </summary>
    /// <typeparam name="TCounter">The type of class that will be used to store the counts.</typeparam>
    public abstract class RequestHandler<TCounter> : IRequestHandler<TCounter>
    {
        private readonly DropperSettings _dropperSettings;

        /// <summary>
        /// Abstract class for handling requests.
        /// </summary><see cref="DropperSettings"/><param name="dropperOptions">.</param>
        protected RequestHandler(IOptions<DropperSettings> dropperOptions)
        {
            this._dropperSettings = dropperOptions.Value;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <param name="key"><see cref="key"/> to find a <see cref="TCounter"/>.</param>
        /// <returns>The <see cref="TCounter"/>.</returns>
        public abstract Task<TCounter> HandleRequestAsync(string key);

        /// <summary>
        /// Increase the <see cref="TCounter"/> for <see cref="key"/> due to rule <see cref="rule"/> being violated.
        /// </summary>
        /// <param name="rule">The <see cref="rule"/> the request has violated.</param>
        /// <param name="key">The <see cref="key"/> that voilated the <see cref="rule"/>.</param>
        /// <returns></returns>
        public abstract Task IncrementCounterAsync(DropperRule rule, string key);

        /// <summary>
        /// Check if the quotaExceeded
        /// </summary>
        /// <param name="counter">The <see cref="TCounter"/> to be checked.</param>
        /// <returns>True if the <see cref="TCounter"/> exceeded the limit given in <see cref="DropperSettings"/>.</returns>
        public abstract Task<bool> QuotaExceededAsync(TCounter counter);

        /// <summary>
        /// Get the rule matching the statusCode, returns null if the the <see cref="path"/> is on the rules excludedPath list.
        /// </summary>
        /// <param name="statusCode">The statusCode for the response.</param>
        /// <param name="path">The path for the request.</param>
        /// <returns>The <see cref="DropperRule"/> or null when no rule or the path is ignored.</returns>
        public virtual DropperRule? GetRule(int statusCode, string? path = null)
        {
            if (this._dropperSettings.Rules.TryGetValue(statusCode.ToString(), out var rule))
            {
                if (path != null && rule.ExcludedPaths.Contains(path))
                {
                    return null;
                }
                return rule;
            }

            return null;
        }

        /// <summary>
        /// Get the block period.
        /// </summary>
        /// <returns>The <see cref="TimeSpan"/> how long a <see cref="Key"/> should <see cref="QuotaExceededAsync"/>.</returns>
        public virtual TimeSpan GetPeriod()
        {
            return this._dropperSettings.Period;
        }

        /// <summary>
        /// Get the limit.
        /// </summary>
        /// <returns>The limit.</returns>
        public virtual int GetLimit()
        {
            return this._dropperSettings.Limit;
        }

        /// <summary>
        /// Get the key belonging to the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/>.</param>
        /// <returns>The key that belongs to the <see cref="context"/></returns>
        public virtual string Key(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? IPAddress.None.ToString();
        }

        /// <summary>
        /// Get the message that should be shown while blocked.
        /// </summary>
        /// <returns>The message shown while blocked.</returns>
        public virtual string? GetMessage()
        {
            return this._dropperSettings.Message;
        }

        /// <summary>
        /// Get the location that the blocked <see cref="Key"/> should be redirected to while blocked.
        /// </summary>
        /// <returns>The redirect location.</returns>
        public virtual string? GetRedirect()
        {
            return this._dropperSettings.Redirect;
        }

        /// <summary>
        /// The list of paths that should be excluded from Requestdropper.
        /// </summary>
        /// <returns>The list of excluded paths.</returns>
        public virtual IList<string> GetExcludedPaths()
        {
            return this._dropperSettings.ExcludedPaths;
        }
    }
}
