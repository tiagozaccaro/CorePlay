using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CorePlay.SDK.Models.Controls;
using CorePlay.SDK.Services;
using SDL2;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CorePlay.SDK.Controls
{
    public class ImageGallery : TemplatedControl
    {
        private readonly IGamepadService _gamepadService;

        public static readonly StyledProperty<LayoutMode> LayoutModeProperty =
            AvaloniaProperty.Register<ImageGallery, LayoutMode>(nameof(LayoutMode), LayoutMode.Grid);

        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<ImageGallery, Orientation>(nameof(Orientation), Orientation.Horizontal);

        public static readonly StyledProperty<ObservableCollection<ImageGalleryItem>> ItemsSourceProperty =
            AvaloniaProperty.Register<ImageGallery, ObservableCollection<ImageGalleryItem>>(nameof(ItemsSource), []);

        public static readonly StyledProperty<ImageGalleryItem> SelectedItemProperty =
            AvaloniaProperty.Register<ImageGallery, ImageGalleryItem>(nameof(SelectedItem));

        public static readonly StyledProperty<Size> ImageSizeProperty =
            AvaloniaProperty.Register<ImageGallery, Size>(nameof(ImageSize), new Size(100, 100));

        public static readonly StyledProperty<Stretch> StretchProperty =
                AvaloniaProperty.Register<ImageGallery, Stretch>(nameof(Stretch), Stretch.Uniform);

        public Stretch Stretch
        {
            get => GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public LayoutMode LayoutMode
        {
            get => GetValue(LayoutModeProperty);
            set => SetValue(LayoutModeProperty, value);
        }

        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public ObservableCollection<ImageGalleryItem> ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public ImageGalleryItem SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public Size ImageSize
        {
            get => GetValue(ImageSizeProperty);
            set => SetValue(ImageSizeProperty, value);
        }

        public Geometry ImageClipGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, ImageSize.Width, ImageSize.Height), 8, 8);
            }
        }

        public ImageGallery()
        {
            //_gamepadService = gamepadService;
            Focusable = true;
            ItemsSourceProperty.Changed.Subscribe(OnItemsChanged);
            SubscribeToCollectionChanged(ItemsSource);
        }

        private void OnItemsChanged(AvaloniaPropertyChangedEventArgs<ObservableCollection<ImageGalleryItem>> e)
        {
            SubscribeToCollectionChanged(e.NewValue.Value);
            UpdateSelectedItemOnItemsChange();
        }

        private void SubscribeToCollectionChanged(ObservableCollection<ImageGalleryItem> collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged += OnItemsCollectionChanged;
            }
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSelectedItemOnItemsChange();
        }

        private void UpdateSelectedItemOnItemsChange()
        {
            if (ItemsSource == null || ItemsSource.Count == 0)
            {
                SelectedItem = null;
            }
            else
            {
                if (SelectedItem == null || !ItemsSource.Contains(SelectedItem))
                {
                    SelectedItem = ItemsSource.First();
                }
            }
        }

        public void NavigateImages(NavigationDirection direction)
        {
            var items = ItemsSource?.ToList();
            if (items == null || items.Count == 0) return;

            if (SelectedItem == null)
            {
                SelectedItem = items.First();
                return;
            }

            var currentIndex = items.IndexOf(SelectedItem);
            if (currentIndex == -1) return;

            int newIndex = LayoutMode == LayoutMode.Grid
                ? NavigateGridLayout(direction, currentIndex, items)
                : NavigateListLayout(direction, currentIndex, items);

            if (newIndex >= 0 && newIndex < items.Count)
            {
                SelectedItem = items[newIndex];
            }
        }

        private int NavigateGridLayout(NavigationDirection direction, int currentIndex, List<ImageGalleryItem> items)
        {
            int columns = GetNumberOfColumns();
            return direction switch
            {
                NavigationDirection.Left => (currentIndex - 1 + items.Count) % items.Count,
                NavigationDirection.Right => (currentIndex + 1) % items.Count,
                NavigationDirection.Up => GetPreviousRowIndex(currentIndex, items.Count, columns),
                NavigationDirection.Down => GetNextRowIndex(currentIndex, items.Count, columns),
                _ => currentIndex
            };
        }

        private int NavigateListLayout(NavigationDirection direction, int currentIndex, List<ImageGalleryItem> items)
        {
            return direction switch
            {
                NavigationDirection.Up => Orientation == Orientation.Vertical
                    ? (currentIndex - 1 + items.Count) % items.Count
                    : (currentIndex - 1 + items.Count) % items.Count,
                NavigationDirection.Down => Orientation == Orientation.Vertical
                    ? (currentIndex + 1) % items.Count
                    : (currentIndex + 1) % items.Count,
                NavigationDirection.Left => Orientation == Orientation.Horizontal
                    ? (currentIndex - 1 + items.Count) % items.Count
                    : (currentIndex - 1 + items.Count) % items.Count,
                NavigationDirection.Right => Orientation == Orientation.Horizontal
                    ? (currentIndex + 1) % items.Count
                    : (currentIndex + 1) % items.Count,
                _ => currentIndex
            };
        }

        private int GetNumberOfColumns()
        {
            var containerWidth = Bounds.Width;
            var container = this.GetVisualParent<ScrollViewer>();
            if (container != null)
            {
                containerWidth = container.Bounds.Width;
            }

            return CalculateNumberOfColumns(containerWidth);
        }

        private int CalculateNumberOfColumns(double containerWidth)
        {
            double itemWidth = ImageSize.Width;
            double spacing = 20;

            int numberOfColumns = (int)Math.Floor((containerWidth + spacing) / (itemWidth + spacing));
            return Math.Max(numberOfColumns, 1);
        }

        private static int GetPreviousRowIndex(int currentIndex, int itemCount, int columns)
        {
            int currentRow = currentIndex / columns;
            int previousRow = 0;

            if (itemCount / columns == 0)
            {
                previousRow = currentRow - 1 + itemCount / columns;
            }
            else
            {
                previousRow = (currentRow - 1 + itemCount / columns) % (itemCount / columns);
            }

            return previousRow * columns + currentIndex % columns;
        }

        private static int GetNextRowIndex(int currentIndex, int itemCount, int columns)
        {
            int rows = (itemCount + columns - 1) / columns;
            int currentRow = currentIndex / columns;
            int nextRow = (currentRow + 1) % rows;
            int newIndex = nextRow * columns + currentIndex % columns;
            return newIndex < itemCount ? newIndex : nextRow * columns + currentIndex % columns;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // Request focus when the control is added to the visual tree
            Dispatcher.UIThread.Post(() => Focus(), DispatcherPriority.Background);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SelectedItemProperty)
            {
                FocusOnSelectedItem();
            }
        }

        private void FocusOnSelectedItem()
        {
            // Find the ItemsControl in the visual tree
            var itemsControl = this.FindDescendantOfType<ItemsControl>();
            if (itemsControl == null) return;

            // Get the index of the selected item
            var items = ItemsSource?.ToList();
            if (items == null || SelectedItem == null) return;

            int index = items.IndexOf(SelectedItem);
            if (index == -1) return;

            // Find the container for the selected item
            var container = itemsControl.ContainerFromIndex(index);
            if (container == null) return;

            // Find the Border within the container and focus it
            var border = container.FindDescendantOfType<Border>();
            border?.Focus();
        }
    }
}
