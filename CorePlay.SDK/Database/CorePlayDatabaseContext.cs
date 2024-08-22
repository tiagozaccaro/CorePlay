using CorePlay.SDK.Models.Database;
using LiteDB;
using LiteDB.Async;
using Microsoft.Extensions.Logging;

namespace CorePlay.SDK.Database
{
    public class CorePlayDatabaseContext : IDisposable
    {
        private readonly LiteDatabaseAsync _database;
        private readonly ILogger<CorePlayDatabaseContext> _logger;

        public ILiteCollectionAsync<Game> Games { get; }
        public ILiteCollectionAsync<Platform> Platforms { get; }
        public ILiteCollectionAsync<Source> Sources { get; }

        public CorePlayDatabaseContext(string connectionString, ILogger<CorePlayDatabaseContext> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                // Re-use mapper from global instance
                var mapper = BsonMapper.Global;

                // Configure the mapper for your entities
                mapper.Entity<Game>()
                    .DbRef(x => x.Platforms, "platforms")
                    .DbRef(x => x.Source, "sources");

                // Initialize LiteDatabaseAsync
                _database = new LiteDatabaseAsync(connectionString);

                Games = _database.GetCollection<Game>("games");
                Platforms = _database.GetCollection<Platform>("platforms");
                Sources = _database.GetCollection<Source>("sources");

                _logger.LogInformation("CorePlayDatabaseContext initialized with connection string.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize CorePlayDatabaseContext.");
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                if (_database != null)
                {
                    _database.Dispose();
                    _logger.LogInformation("CorePlayDatabaseContext disposed asynchronously.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispose CorePlayDatabaseContext asynchronously.");
                throw;
            }
        }
    }
}
