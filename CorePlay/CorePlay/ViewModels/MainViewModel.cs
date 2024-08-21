using CorePlay.Helpers;
using CorePlay.Models;
using CorePlay.SDK.Database;
using CorePlay.SDK.Providers;
using CorePlay.SDK.Models;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CorePlay.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly CorePlayDatabaseContext _database;
        private readonly IEnumerable<ILibraryProvider> _libraryProviders;
        private readonly IEnumerable<IMetadataProvider> _metadataProviders;

        public ObservableCollection<ImageListItem> Items { get; } = [];
        public ObservableCollection<ImageListItem> Platforms { get; } = [];

        public MainViewModel() { }

        // Constructor for actual use
        public MainViewModel(CorePlayDatabaseContext database, IEnumerable<ILibraryProvider> libraryProviders, IEnumerable<IMetadataProvider> metadataProviders)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _libraryProviders = libraryProviders ?? throw new ArgumentNullException(nameof(libraryProviders));
            _metadataProviders = metadataProviders ?? throw new ArgumentNullException(nameof(metadataProviders));
            LoadGamesFromDatabaseAsync().ConfigureAwait(false);
        }

        private async Task LoadPlatformsAsync(CancellationToken cancellationToken)
        {
            await LoadPlatformsFromDatabaseAsync();

            string directoryPath = @"D:/Documents/CorePlay/deploy/plugins/Assets/Platforms/Light - Color"; // Change this to your target directory path

            try
            {
                // Get all files in the directory and subdirectories
                string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

                foreach (string fileName in files)
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName).Trim();

                    var dbPlatform = await _database.Platforms.Query()
                        .Where(g => g.Name == fileNameWithoutExtension).FirstOrDefaultAsync();

                    if (dbPlatform == null && fileNameWithoutExtension.Length > 0)
                    {
                        dbPlatform = new Platform
                        {
                            Id = Guid.NewGuid(),
                            Logo = fileName,
                            Name = fileNameWithoutExtension,
                        };

                        await _database.Platforms.InsertAsync(dbPlatform);
                        Console.WriteLine(dbPlatform.Name);

                        // if (!Platforms.Any(p => p.FallbackText == fileNameWithoutExtension))
                        // {
                        //     Platforms.Add(new ImageListItem
                        //     {
                        //         FallbackText = fileNameWithoutExtension,
                        //         ImageSource = fileName
                        //     });
                        // }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //var tasks = _metadataProviders.Select(metadata => LoadPlatformsFromProviderAsync(metadata, cancellationToken)).ToArray();
            //await Task.WhenAll(tasks);
        }

        private async Task LoadGamesAsync(CancellationToken cancellationToken)
        {
            await LoadGamesFromDatabaseAsync();
            var tasks = _libraryProviders.Select(library => LoadGamesFromProviderAsync(library, cancellationToken)).ToArray();
            await Task.WhenAll(tasks);
        }

        private async Task LoadGamesFromProviderAsync(ILibraryProvider libraryProvider, CancellationToken cancellationToken)
        {
            try
            {
                var games = await libraryProvider.GetGamesAsync();
                var platforms = await _database.Platforms.Query().OrderBy(p => p.Name).ToListAsync();
                var platformsHashSet = new List<ImageListItem>();

                foreach (var game in games)
                {
                    var dbGame = await _database.Games.Query().Include(x => x.Platforms).OrderBy(g => g.Name).Where(g => g.GameId == game.Id).FirstOrDefaultAsync();

                    if (dbGame == null)
                    {
                        dbGame = new Game
                        {
                            GameId = game.Id,
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

                        await _database.Games.InsertAsync(dbGame);
                    }

                    foreach (var metadataProvider in _metadataProviders)
                    {
                        try
                        {
                            var metaGame = await metadataProvider.GetGameMetadataByNameAsync(game.Name, cancellationToken);

                            if (metaGame != null)
                            {
                                dbGame.Cover ??= metaGame.Cover;
                                dbGame.Description ??= metaGame.Description;
                                dbGame.Icon ??= metaGame.Icon;
                                dbGame.LastActivity ??= metaGame.LastActivity;
                                dbGame.Logo ??= metaGame.Logo;
                                dbGame.Name ??= metaGame.Name;
                                dbGame.Playtime = dbGame.Playtime == 0 ? metaGame.Playtime : dbGame.Playtime;
                                dbGame.Videos ??= metaGame.Videos;

                                foreach (var platform in platforms.Where(p => metaGame.Platforms.Any(meta => p.Name.Equals(((MetadataNameProperty)meta).Name, StringComparison.InvariantCultureIgnoreCase))).ToHashSet())
                                {
                                    dbGame.Platforms.Add(platform);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }

                    await _database.Games.UpdateAsync(dbGame);

                    if (!Items.Any(p => p.FallbackText == game?.Name))
                    {
                        Items.Add(new ImageListItem
                        {
                            FallbackText = dbGame.Name,
                            ImageSource = dbGame.Cover
                        });
                    }

                    foreach (var platform in dbGame.Platforms)
                    {
                        platformsHashSet.Add(new ImageListItem
                        {
                            FallbackText = platform.Name,
                            ImageSource = platform.Logo
                        });
                    }
                }

                Platforms.AddRange(platformsHashSet.Where(d => !platformsHashSet.Any(p => p.FallbackText == d.FallbackText)).DistinctBy(k => k.FallbackText));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task LoadGamesFromDatabaseAsync()
        {
            var games = await _database.Games
                .Include(x => x.Platforms)
                .Query()
                .OrderBy(g => g.Name)
                .ToListAsync();

            Items.AddRange(games
                .Select(x => new ImageListItem
                {
                    FallbackText = x.Name,
                    ImageSource = x.Cover
                }));

            Platforms.AddRange(games
                .SelectMany(x => x.Platforms)
                .DistinctBy(x => x.Id)
                .Select(p => new ImageListItem
                {
                    FallbackText = p.Name,
                    ImageSource = p.Logo
                }));
        }

        private async Task LoadPlatformsFromProviderAsync(IMetadataProvider metadataProvider, CancellationToken cancellationToken)
        {
            try
            {
                var platforms = await metadataProvider.GetAllPlatformMetadataAsync(cancellationToken);

                foreach (var platform in platforms)
                {
                    var dbPlatform = await _database.Platforms.Query().Where(g => g.Name == platform.Name).FirstOrDefaultAsync();

                    if (dbPlatform == null)
                    {
                        dbPlatform = new Platform
                        {
                            Id = Guid.NewGuid(),
                            Description = platform.Description,
                            Logo = platform.Logo,
                            Name = platform.Name,
                        };

                        await _database.Platforms.InsertAsync(dbPlatform);
                    }

                    if (!Platforms.Any(p => p.FallbackText == platform?.Name))
                    {
                        Platforms.Add(new ImageListItem
                        {
                            FallbackText = dbPlatform.Name,
                            ImageSource = dbPlatform.Logo
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task LoadPlatformsFromDatabaseAsync()
        {
            var platforms = await _database.Platforms.Query().OrderBy(g => g.Name).ToListAsync();
            var platformsHashSet = new List<ImageListItem>();

            foreach (var platform in platforms)
            {
                platformsHashSet.Add(new ImageListItem
                {
                    FallbackText = platform.Name,
                    ImageSource = platform.Logo,
                });
            }

            Platforms.AddRange(platformsHashSet.Distinct());
        }
    }
}
