using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RequestDropper.Extensions
{
    /// <summary>
    /// Helper functions for configuring RequestDropper.
    /// </summary>
    public class RequestDropperBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="RequestDropperBuilder"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance containing the RequestDropper definitions.</param>
        public RequestDropperBuilder(IServiceCollection services, IConfiguration configuration)
        {
            this.Services = services;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceCollection"/> services are attached to.
        /// </value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Gets the <see cref="IConfiguration"/> configuration.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/>.
        /// </value>
        public IConfiguration Configuration { get; }
    }
}
