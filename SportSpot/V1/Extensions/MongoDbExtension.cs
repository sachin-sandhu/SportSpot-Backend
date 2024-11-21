using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using SportSpot.V1.Database;
using SportSpot.V1.Media.Entities;
using SportSpot.V1.Session.Entities;

namespace SportSpot.V1.Extensions
{
    public static class MongoDbExtension
    {

        public static async Task AddMongoDb(this IServiceCollection services, string connectionString, string database)
        {
            MongoClient client = new(connectionString);
            IMongoDatabase db = client.GetDatabase(database);

            ConventionPack pack = [new GuidRepresentationConvention()];
            ConventionRegistry.Register("Guid Convention", pack, type => true);

            services.AddSingleton(client);
            services.AddSingleton(db);

            // Create the collections
            IMongoCollection<MediaEntity> mediaCollection = db.GetCollection<MediaEntity>("Media");
            IMongoCollection<SessionEntity> sessionCollection = db.GetCollection<SessionEntity>("Session");

            // Add custom Indexes
            CreateIndexModel<SessionEntity> geoIndex = new(Builders<SessionEntity>.IndexKeys.Geo2DSphere(x => x.Location.Coordinates));
            await sessionCollection.Indexes.CreateOneAsync(geoIndex);

            // Register the collections
            services.AddSingleton(mediaCollection);
            services.AddSingleton(sessionCollection);
        }
    }
}
