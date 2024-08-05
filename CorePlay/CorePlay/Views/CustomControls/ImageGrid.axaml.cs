using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CorePlay.Models;
using System.Collections.ObjectModel;

namespace CorePlay.Views.CustomControls;

public class ImageGrid : TemplatedControl
{
    public static readonly StyledProperty<string> LayoutModeProperty =
        AvaloniaProperty.Register<ImageGrid, string>(nameof(LayoutMode), "Grid");

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<ImageGrid, Orientation>(nameof(Orientation));

    public static readonly StyledProperty<ObservableCollection<ImageGridItem>> ItemsProperty =
            AvaloniaProperty.Register<ImageGrid, ObservableCollection<ImageGridItem>>(nameof(Items));

    public string LayoutMode
    {
        get => GetValue(LayoutModeProperty);
        set => SetValue(LayoutModeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }


    public ObservableCollection<ImageGridItem> Items
    {
        get => GetValue(ItemsProperty);
        private set => SetValue(ItemsProperty, value);
    }
}