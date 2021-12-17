using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using RequestDropper.Handlers;
using RequestDropper.Models;
using RequestDropper.Stores;

namespace RequestDropper.Extensions
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Adds the default request dropper.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="configuration">The configuration available in the application.</param>
        /// <returns>An <see cref="RequestDropperBuilder"/> for creating and configuring the request dropper.</returns>
        public static RequestDropperBuilder AddRequestDropper(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DropperSettings>(configuration.GetSection(nameof(DropperSettings)));
            services.AddLogging();
            return new RequestDropperBuilder(services, configuration);
        }

        /// <summary>
        /// Adds an <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>.
        /// </summary>
        /// <param name="builder">The requestbuilder created by <see cref="AddRequestDropper"/>.</param>
        /// <returns>The current <see cref="RequestDropperBuilder"/> instance.</returns>
        public static RequestDropperBuilder AddDistributedCacheStore(this RequestDropperBuilder builder)
        {
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton<NativeRequestHandler>();
            builder.Services.TryAddSingleton<IStore<DropCounter?>, DistributedCacheStore<DropCounter?>>();
            return builder;
        }

        /// <summary>
        /// Adds an <see cref="Microsoft.Extensions.Caching.Memory.IMemoryCache"/>.
        /// </summary>
        /// <param name="builder">The requestbuilder created by <see cref="AddRequestDropper"/>.</param>
        /// <returns>The current <see cref="RequestDropperBuilder"/> instance.</returns>
        public static RequestDropperBuilder AddMemoryCacheStore(this RequestDropperBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<NativeRequestHandler>();
            builder.Services.TryAddSingleton<IStore<DropCounter?>, MemoryCacheStore<DropCounter?>>();
            return builder;
        }
    }
}
