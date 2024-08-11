using CorePlay.Helpers;
using CorePlay.Models;
using CorePlay.SDK.Database;
using CorePlay.SDK.Interfaces.Providers;
using CorePlay.SDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CorePlay.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly CorePlayDatabaseContext _database;
        private readonly IEnumerable<ILibraryProvider> _libraryProviders;
        private readonly IEnumerable<IMetadataProvider> _metadataProviders;

        public ObservableCollection<ImageGridItem> Items { get; } = [];

        // Constructor for actual use
        public MainViewModel(CorePlayDatabaseContext database, IEnumerable<ILibraryProvider> libraryProviders, IEnumerable<IMetadataProvider> metadataProviders)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _libraryProviders = libraryProviders ?? throw new ArgumentNullException(nameof(libraryProviders));
            _metadataProviders = metadataProviders ?? throw new ArgumentNullException(nameof(metadataProviders));
            LoadGamesAsync().ConfigureAwait(false);
        }

        private async Task LoadGamesAsync()
        {
            var tasks = _libraryProviders.Select(LoadGamesFromProviderAsync).ToArray();
            await Task.WhenAll(tasks);
            await LoadGamesFromDatabaseAsync();
        }

        private async Task LoadGamesFromProviderAsync(ILibraryProvider libraryProvider)
        {
            try
            {
                var games = await libraryProvider.GetGamesAsync();

                foreach (var game in games)
                {
                    var dbGame = await _database.Games.Query().Where(g => g.GameId == game.GameId).FirstOrDefaultAsync();

                    if (dbGame == null)
                    {
                        dbGame = new Game
                        {
                            GameId = game.GameId,
                            Artworks = game.Artworks,
                            Cover = game.Cover,
                            Description = game.Description,
                            Icon = game.Icon,
                            LastActivity = game.LastActivity,
                            Logo = game.Logo,
                            Name = game.Name,
                            Playtime = game.Playtime,
                            Videos = game.Videos,
                        };

                        foreach (var metadataProvider in _metadataProviders)
                        {
                            var metaGame = await metadataProvider.GetGameMetadataAsync(game.Name);

                            if (metaGame != null)
                            {
                                dbGame.Cover = metaGame.Cover;
                                dbGame.Description = metaGame.Description;
                                dbGame.Icon = metaGame.Icon;
                                dbGame.LastActivity = metaGame.LastActivity;
                                dbGame.Logo = metaGame.Logo;
                                dbGame.Name = metaGame.Name;
                                dbGame.Playtime = metaGame.Playtime;
                                dbGame.Videos = metaGame.Videos;
                            }
                        }

                        await _database.Games.InsertAsync(dbGame);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
            }
        }

        private async Task LoadGamesFromDatabaseAsync()
        {
            var games = await _database.Games.Query().OrderBy(g => g.Name).ToListAsync();

            foreach (var game in games)
            {
                //var imageSource = game.Cover != null ? await ImageHelper.LoadFromWeb(new Uri(game.Cover)) : null;

                Items.Add(new ImageGridItem
                {
                    FallbackText = game.Name,
                    ImageSource = game.Cover,
                });
            }
        }
    }
}
