using Microsoft.AspNetCore.Http;

namespace RequestDropper.CounterKeyBuilders
{
    public interface ICounterKeyBuilder
    {
        string Build(HttpContext context);
    }
}
