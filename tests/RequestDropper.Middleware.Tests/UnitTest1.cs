using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NUnit.Framework;

using RequestDropper.Extensions;

namespace RequestDropper.Middleware.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task RequestDropperMiddleware_Returns429OnceLimitIsExceeded()
        {
            var config = new ConfigurationBuilder()
                .Add(new MemoryConfigurationSource()
                {
                    InitialData = new List<KeyValuePair<string, string>>
                                               {
                                                   new("DropperSettings:Limit", 30.ToString()),
                                                   new("DropperSettings:Period", "00:01:00"),
                                                   new("DropperSettings:Rules:404:Weight", 15.ToString()),
                                               }
                })
                .Build();

            using var host = await new HostBuilder()
                                 .ConfigureWebHost(webBuilder =>
                                     {

                                         webBuilder
                                             .UseTestServer()
                                             .ConfigureServices(services =>
                                             {
                                                 services.AddControllers();
                                                 services.AddRequestDropper(config).AddMemoryCacheStore();
                                                 })
                                             .Configure(app =>
                                                 {
                                                     app.UseRequestDropperMiddleware();
                                                     app.UseEndpoints(endpoints =>
                                                         {
                                                             endpoints.MapControllers();
                                                         });
                                                 });
                                     })
                                 .StartAsync();
            
            var response1 = await host.GetTestClient().GetAsync("/404");
            var response2 = await host.GetTestClient().GetAsync("/404");
            var response3 = await host.GetTestClient().GetAsync("/404");

            Assert.AreEqual(HttpStatusCode.NotFound, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, response2.StatusCode);
            Assert.AreEqual(HttpStatusCode.TooManyRequests, response3.StatusCode);
        }
    }
}