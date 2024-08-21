using Avalonia.Controls;
using Avalonia.Interactivity;

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
            Overlay.IsVisible = !Overlay.IsVisible;
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            ToggleDialog();
        }
    }
}