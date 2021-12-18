using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        public async Task RequestDropperMiddleware_Returns429OnceLimitIsExceeded_WithMemoryCacheStore()
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
                                                     app.UseRouting();
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

        [Test]
        public async Task RequestDropperMiddleware_Returns429OnceLimitIsExceeded_WithDistributedCacheStore()
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
                                             services.AddRequestDropper(config).AddDistributedCacheStore();
                                         })
                                         .Configure(app =>
                                         {
                                             app.UseRequestDropperMiddleware();
                                             app.UseRouting();
                                             app.UseEndpoints(endpoints =>
                                             {
                                                 endpoints.MapControllers();
                                             });
                                         });
                                 })
                                 .StartAsync();

            var response1 = await host.GetTestClient().GetAsync("/index/404");
            var response2 = await host.GetTestClient().GetAsync("/index/404");
            var response3 = await host.GetTestClient().GetAsync("/index/404");

            Assert.AreEqual(HttpStatusCode.NotFound, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, response2.StatusCode);
            Assert.AreEqual(HttpStatusCode.TooManyRequests, response3.StatusCode);
        }

        [Test]
        public async Task RequestDropperMiddleware_IgnoresGlobalExcludedPath()
        {

            var config = new ConfigurationBuilder()
                .Add(new MemoryConfigurationSource()
                {
                    InitialData = new List<KeyValuePair<string, string>>
                                               {
                                                   new("DropperSettings:Limit", 10.ToString()),
                                                   new("DropperSettings:Period", "00:01:00"),
                                                   new("DropperSettings:Rules:404:Weight", 15.ToString()),
                                                   new("DropperSettings:ExcludedPaths:0", "/home/200"),
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
                                             services.AddRequestDropper(config).AddDistributedCacheStore();
                                         })
                                         .Configure(app =>
                                         {
                                             app.UseRequestDropperMiddleware();
                                             app.UseRouting();
                                             app.UseEndpoints(endpoints =>
                                             {
                                                 endpoints.MapControllers();
                                             });
                                         });
                                 })
                                 .StartAsync();

            var response1 = await host.GetTestClient().GetAsync("/index/404");
            var response2 = await host.GetTestClient().GetAsync("/index/404");
            var response3 = await host.GetTestClient().GetAsync("/home/200");

            Assert.AreEqual(HttpStatusCode.NotFound, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.TooManyRequests, response2.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response3.StatusCode);
        }

        [Test]
        public async Task RequestDropperMiddleware_IgnoresRulesExcludedPath()
        {

            var config = new ConfigurationBuilder()
                .Add(new MemoryConfigurationSource()
                {
                    InitialData = new List<KeyValuePair<string, string>>
                                               {
                                                   new("DropperSettings:Limit", 10.ToString()),
                                                   new("DropperSettings:Period", "00:01:00"),
                                                   new("DropperSettings:Rules:404:Weight", 15.ToString()),
                                                   new("DropperSettings:Rules:404:ExcludedPaths:0", "/home/404"),
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
                                             services.AddRequestDropper(config).AddDistributedCacheStore();
                                         })
                                         .Configure(app =>
                                         {
                                             app.UseRequestDropperMiddleware();
                                             app.UseRouting();
                                             app.UseEndpoints(endpoints =>
                                             {
                                                 endpoints.MapControllers();
                                             });
                                         });
                                 })
                                 .StartAsync();

            var response1 = await host.GetTestClient().GetAsync("/home/404");
            var response2 = await host.GetTestClient().GetAsync("/index/200");

            Assert.AreEqual(HttpStatusCode.NotFound, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
        }
        
        [Test]
        public async Task RequestDropperMiddleware_ReturnsCustomMessage()
        {

            var config = new ConfigurationBuilder()
                .Add(new MemoryConfigurationSource()
                {
                    InitialData = new List<KeyValuePair<string, string>>
                                               {
                                                   new("DropperSettings:Limit", 10.ToString()),
                                                   new("DropperSettings:Period", "00:01:00"),
                                                   new("DropperSettings:Rules:404:Weight", 15.ToString()),
                                                   new("DropperSettings:Message", "This is your custom message!"),
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
                                             services.AddRequestDropper(config).AddDistributedCacheStore();
                                         })
                                         .Configure(app =>
                                         {
                                             app.UseRequestDropperMiddleware();
                                             app.UseRouting();
                                             app.UseEndpoints(endpoints =>
                                             {
                                                 endpoints.MapControllers();
                                             });
                                         });
                                 })
                                 .StartAsync();

            var response1 = await host.GetTestClient().GetAsync("/index/404");
            var response2 = await host.GetTestClient().GetAsync("/index/404");

            Assert.AreEqual(HttpStatusCode.NotFound, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.TooManyRequests, response2.StatusCode);
            Assert.AreEqual(await response2.Content.ReadAsStringAsync(), "This is your custom message!");
        }

        [Test]
        public async Task RequestDropperMiddleware_ReturnsRedirect()
        {

            var config = new ConfigurationBuilder()
                .Add(new MemoryConfigurationSource()
                {
                    InitialData = new List<KeyValuePair<string, string>>
                                               {
                                                   new("DropperSettings:Limit", 10.ToString()),
                                                   new("DropperSettings:Period", "00:01:00"),
                                                   new("DropperSettings:Rules:404:Weight", 15.ToString()),
                                                   new("DropperSettings:Redirect", "https://github.com/"),
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
                                             services.AddRequestDropper(config).AddDistributedCacheStore();
                                         })
                                         .Configure(app =>
                                         {
                                             app.UseRequestDropperMiddleware();
                                             app.UseRouting();
                                             app.UseEndpoints(endpoints =>
                                             {
                                                 endpoints.MapControllers();
                                             });
                                         });
                                 })
                                 .StartAsync();

            var response1 = await host.GetTestClient().GetAsync("/index/404");
            var response2 = await host.GetTestClient().GetAsync("/index/404");

            Assert.AreEqual(HttpStatusCode.NotFound, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.Redirect, response2.StatusCode);
            Assert.AreEqual(response2.Headers.Location, "https://github.com/");
        }

        //[Test]
        //public async Task RequestDropperMiddleware_Returns429OnceLimitIsExceeded_WithMongoStore()
        //{
        //    var config = new ConfigurationBuilder()
        //        .Add(new MemoryConfigurationSource()
        //        {
        //            InitialData = new List<KeyValuePair<string, string>>
        //                                       {
        //                                           new("DropperSettings:Limit", 30.ToString()),
        //                                           new("DropperSettings:Period", "00:01:00"),
        //                                           new("DropperSettings:Rules:404:Weight", 15.ToString()),

        //                                           new("MongoSettings:ConnectionString", "mongodb://1.1.1.1"),
        //                                           new("MongoSettings:Database", "database")
        //                                       }
        //        })
        //        .Build();

        //    using var host = await new HostBuilder()
        //                         .ConfigureWebHost(webBuilder =>
        //                         {

        //                             webBuilder
        //                                 .UseTestServer()
        //                                 .ConfigureServices(services =>
        //                                 {
        //                                     services.AddControllers();
        //                                     services.AddRequestDropper(config).AddMongoCacheStore();
        //                                 })
        //                                 .Configure(app =>
        //                                 {
        //                                     app.UseRequestDropperMiddleware<MongoDropCounter>();
        //                                     app.UseRouting();
        //                                     app.UseEndpoints(endpoints =>
        //                                     {
        //                                         endpoints.MapControllers();
        //                                     });
        //                                 });
        //                         })
        //                         .StartAsync();

        //    var response1 = await host.GetTestClient().GetAsync("/404");
        //    var response2 = await host.GetTestClient().GetAsync("/404");
        //    var response3 = await host.GetTestClient().GetAsync("/404");

        //    Assert.AreEqual(HttpStatusCode.NotFound, response1.StatusCode);
        //    Assert.AreEqual(HttpStatusCode.NotFound, response2.StatusCode);
        //    Assert.AreEqual(HttpStatusCode.TooManyRequests, response3.StatusCode);
        //}
    }
}