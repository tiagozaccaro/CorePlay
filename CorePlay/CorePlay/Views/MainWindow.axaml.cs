using Avalonia;
using Avalonia.Controls;

namespace CorePlay.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Get the primary screen's bounds
            var screenBounds = Screens.Primary.Bounds;

            // Set the window to fullscreen with the exact screen resolution
            this.WindowState = WindowState.FullScreen;
            this.SystemDecorations = SystemDecorations.None;
            this.Width = screenBounds.Width;
            this.Height = screenBounds.Height;

            // Ensure the window starts at the top-left corner of the screen
            this.Position = new PixelPoint(0, 0);
        }
    }
}