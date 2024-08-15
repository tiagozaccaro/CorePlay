using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using CorePlay.Enums;
using CorePlay.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace CorePlay.Controls
{
    public class ImageGrid : TemplatedControl
    {
        public static readonly StyledProperty<LayoutMode> LayoutModeProperty =
            AvaloniaProperty.Register<ImageGrid, LayoutMode>(nameof(LayoutMode), LayoutMode.Grid);

        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<ImageGrid, Orientation>(nameof(Orientation), Orientation.Horizontal);

        public static readonly StyledProperty<ObservableCollection<ImageListItem>> ItemsSourceProperty =
            AvaloniaProperty.Register<ImageGrid, ObservableCollection<ImageListItem>>(nameof(ItemsSource), []);

        public static readonly StyledProperty<ImageListItem> SelectedItemProperty =
            AvaloniaProperty.Register<ImageGrid, ImageListItem>(nameof(SelectedItem));

        public static readonly StyledProperty<Size> ImageSizeProperty =
            AvaloniaProperty.Register<ImageGrid, Size>(nameof(ImageSize), new Size(100, 100));

        public static readonly StyledProperty<Key> MoveLeftKeyProperty =
            AvaloniaProperty.Register<ImageGrid, Key>(nameof(MoveLeftKey), Key.Left);

        public static readonly StyledProperty<Key> MoveRightKeyProperty =
            AvaloniaProperty.Register<ImageGrid, Key>(nameof(MoveRightKey), Key.Right);

        public static readonly StyledProperty<Key> MoveUpKeyProperty =
            AvaloniaProperty.Register<ImageGrid, Key>(nameof(MoveUpKey), Key.Up);

        public static readonly StyledProperty<Key> MoveDownKeyProperty =
            AvaloniaProperty.Register<ImageGrid, Key>(nameof(MoveDownKey), Key.Down);

        public static readonly StyledProperty<Stretch> StretchProperty =
                AvaloniaProperty.Register<ImageGrid, Stretch>(nameof(Stretch), Stretch.Uniform);

        public Stretch Stretch
        {
            get => GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public Key MoveLeftKey
        {
            get => GetValue(MoveLeftKeyProperty);
            set => SetValue(MoveLeftKeyProperty, value);
        }

        public Key MoveRightKey
        {
            get => GetValue(MoveRightKeyProperty);
            set => SetValue(MoveRightKeyProperty, value);
        }

        public Key MoveUpKey
        {
            get => GetValue(MoveUpKeyProperty);
            set => SetValue(MoveUpKeyProperty, value);
        }

        public Key MoveDownKey
        {
            get => GetValue(MoveDownKeyProperty);
            set => SetValue(MoveDownKeyProperty, value);
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

        public ObservableCollection<ImageListItem> ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public ImageListItem SelectedItem
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

        public ImageGrid()
        {
            Focusable = true;
            ItemsSourceProperty.Changed.Subscribe(OnItemsChanged);
            SubscribeToCollectionChanged(ItemsSource);
        }

        private void OnItemsChanged(AvaloniaPropertyChangedEventArgs<ObservableCollection<ImageListItem>> e)
        {
            SubscribeToCollectionChanged(e.NewValue.Value);
            UpdateSelectedItemOnItemsChange();
        }

        private void SubscribeToCollectionChanged(ObservableCollection<ImageListItem> collection)
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            Key key = e.Key;

            if (key == MoveLeftKey || key == MoveRightKey || key == MoveUpKey || key == MoveDownKey)
            {
                if (SelectedItem == null)
                {
                    if (ItemsSource != null && ItemsSource.Any())
                    {
                        SelectedItem = ItemsSource.First();
                    }
                }
                else
                {
                    NavigateImages(key);
                }

                e.Handled = true;
            }
        }

        private void NavigateImages(Key key)
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

            int newIndex = -1;
            int columns = GetNumberOfColumns();

            if (LayoutMode == LayoutMode.Grid)
            {
                newIndex = NavigateGridLayout(key, currentIndex, items, columns);
            }
            else
            {
                newIndex = NavigateListLayout(key, currentIndex, items);
            }

            if (newIndex >= 0 && newIndex < items.Count)
            {
                SelectedItem = items[newIndex];
            }
        }

        private static int NavigateGridLayout(Key key, int currentIndex, List<ImageListItem> items, int columns)
        {
            int newIndex = currentIndex;

            switch (key)
            {
                case Key.Left:
                    newIndex = (currentIndex - 1 + items.Count) % items.Count;
                    break;
                case Key.Right:
                    newIndex = (currentIndex + 1) % items.Count;
                    break;
                case Key.Up:
                    newIndex = GetPreviousRowIndex(currentIndex, items.Count, columns);
                    break;
                case Key.Down:
                    newIndex = GetNextRowIndex(currentIndex, items.Count, columns);
                    break;
            }

            return newIndex;
        }

        private int NavigateListLayout(Key key, int currentIndex, List<ImageListItem> items)
        {
            int newIndex = currentIndex;

            if (Orientation == Orientation.Vertical)
            {
                switch (key)
                {
                    case Key.Up:
                        newIndex = (currentIndex - 1 + items.Count) % items.Count;
                        break;
                    case Key.Down:
                        newIndex = (currentIndex + 1) % items.Count;
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    case Key.Left:
                        newIndex = (currentIndex - 1 + items.Count) % items.Count;
                        break;
                    case Key.Right:
                        newIndex = (currentIndex + 1) % items.Count;
                        break;
                }
            }

            return newIndex;
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
                previousRow = currentRow - 1 + (itemCount / columns);
            }
            else
            {
                previousRow = (currentRow - 1 + (itemCount / columns)) % (itemCount / columns);
            }

            return previousRow * columns + (currentIndex % columns);
        }

        private static int GetNextRowIndex(int currentIndex, int itemCount, int columns)
        {
            int rows = (itemCount + columns - 1) / columns;
            int currentRow = currentIndex / columns;
            int nextRow = (currentRow + 1) % rows;
            int newIndex = nextRow * columns + (currentIndex % columns);
            return newIndex < itemCount ? newIndex : (nextRow * columns + (currentIndex % columns));
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // Request focus when the control is added to the visual tree
            this.Focus();
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
