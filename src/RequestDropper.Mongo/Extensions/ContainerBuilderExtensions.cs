using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using RequestDropper.Extensions;
using RequestDropper.Mongo.Handlers;
using RequestDropper.Mongo.Models;
using RequestDropper.Mongo.Settings;
using RequestDropper.Mongo.Stores;
using RequestDropper.Stores;

namespace RequestDropper.Mongo.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static RequestDropperBuilder AddMongoCacheStore(this RequestDropperBuilder services)
        {
            services.Services.Configure<MongoSettings>(services.Configuration.GetSection(nameof(MongoSettings)));
            var mongoSettings = services.Configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>() ?? throw new ArgumentException($"No '{nameof(MongoSettings)}' settings found in the configuration file", nameof(services.Configuration));

            var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);

            services.Services.AddSingleton(s =>
                {
                    var client = new MongoClient(settings);
                    return client;
                });

            services.Services.AddSingleton(s =>
                {
                    var client = s.GetRequiredService<MongoClient>();
                    var database = client.GetDatabase(mongoSettings.Database);
                    BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance);

                    var pack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
                    ConventionRegistry.Register("Ignore extra elements", pack, x => true);

                    return database;
                });

            services.Services.AddSingleton<MongoRequestHandler>();
            services.Services.TryAddSingleton<IStore<MongoDropCounter>, MongoCacheStore<MongoDropCounter>>();
            return services;
        }
    }
}
