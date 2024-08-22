using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CorePlay.SDK.Services;
using SDL2;
using System.Collections.Generic;

namespace CorePlay.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void ToggleDialog()
        {
            bool isVisible = !Overlay.IsVisible;
            Overlay.IsVisible = isVisible;
            Dialog.IsVisible = isVisible; // Toggle visibility of the OptionsDialog as well
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            ToggleDialog();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var gamesDirectionMap = new Dictionary<Key, NavigationDirection>
            {
                { Key.Left, NavigationDirection.Left },
                { Key.Right, NavigationDirection.Right },
                { Key.Up, NavigationDirection.Up },
                { Key.Down, NavigationDirection.Down }
            };

            if (gamesDirectionMap.TryGetValue(e.Key, out var gamesDirection))
            {
                GamesGallery.NavigateImages(gamesDirection);
                e.Handled = true;
                return;
            }

            var platformsDirectionMap = new Dictionary<Key, NavigationDirection>
            {
                { Key.A, NavigationDirection.Left },
                { Key.D, NavigationDirection.Right }
            };

            if (platformsDirectionMap.TryGetValue(e.Key, out var platformsDirection))
            {
                PlatformsGallery.NavigateImages(platformsDirection);
                e.Handled = true;
            }
        }
    }
}
