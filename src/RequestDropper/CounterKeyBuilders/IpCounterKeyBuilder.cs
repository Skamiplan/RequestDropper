using Microsoft.AspNetCore.Http;

namespace RequestDropper.CounterKeyBuilders
{
    internal class IpCounterKeyBuilder : ICounterKeyBuilder
    {
        #region Implementation of ICounterKeyBuilder

        public string Build(HttpContext context)
        {
            return context.Connection.RemoteIpAddress.ToString();
        }

        #endregion
    }
}
