using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using RequestDropper.Models;

namespace RequestDropper.Handlers
{
    public interface IRequestHandler<T>
    {
        /// <summary>
        /// Get the <see cref="T"/> that belongs to the <see cref="key"/>
        /// </summary>
        /// <param name="key"><see cref="key"/></param>
        /// <returns>The <see cref="DropCounter"/> belonging to the <see cref="key"/></returns>
        Task<T> HandleRequestAsync(string key);

        /// <summary>
        /// Increase the <see cref="T"/> belonging to the <see cref="key"/>
        /// </summary>
        /// <param name="rule"><see cref="DropperRule"/></param>
        /// <param name="key"><see cref="key"/></param>
        /// <returns></returns>
        Task IncrementCounterAsync(DropperRule rule, string key);

        /// <summary>
        /// Check if the quotaExceeded
        /// </summary>
        /// <param name="counter">The <see cref="T"/> to be checked.</param>
        /// <returns>True if the <see cref="T"/> exceeded the limit given in <see cref="DropperSettings"/>.</returns>
        Task<bool> QuotaExceededAsync(T counter);

        /// <summary>
        /// Get the rule for the corresponding parameters.
        /// </summary>
        /// <param name="statusCode">The statusCode for the response.</param>
        /// <param name="path">The path for the request.</param>
        /// <returns>The <see cref="DropperRule"/> for the corresponding parameters.</returns>
        DropperRule? GetRule(int statusCode, string? path);

        /// <summary>
        /// Get the block period.
        /// </summary>
        /// <returns>The <see cref="TimeSpan"/> how long a <see cref="Key"/> should <see cref="QuotaExceededAsync"/>.</returns>
        TimeSpan GetPeriod();
        
        /// <summary>
        /// Get the limit.
        /// </summary>
        /// <returns>The limit.</returns>
        int GetLimit();

        /// <summary>
        /// Get the key belonging to the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/>.</param>
        /// <returns>The key that belongs to the <see cref="context"/></returns>
        string Key(HttpContext context);
        
        /// <summary>
        /// Get the message that should be shown while blocked.
        /// </summary>
        /// <returns>The message shown while blocked.</returns>
        string? GetMessage();
        
        /// <summary>
        /// Get the location that the blocked <see cref="Key"/> should be redirected to while blocked.
        /// </summary>
        /// <returns>The redirect location.</returns>
        string? GetRedirect();
        
        /// <summary>
        /// The list of paths that should be excluded from Requestdropper.
        /// </summary>
        /// <returns>The list of excluded paths.</returns>
        IList<string> GetExcludedPaths();
    }
}