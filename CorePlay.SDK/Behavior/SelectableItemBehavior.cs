using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CorePlay.SDK.Models.Controls;
using System;

namespace CorePlay.SDK.Behaviors
{
    public static class SelectableItemBehavior
    {
        public static readonly AttachedProperty<object> ItemProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, object>("Item", typeof(SelectableItemBehavior));

        public static void SetItem(AvaloniaObject element, object value) => element.SetValue(ItemProperty, value);
        public static object GetItem(AvaloniaObject element) => element.GetValue(ItemProperty);

        static SelectableItemBehavior()
        {
            ItemProperty.Changed.AddClassHandler<Control>((control, args) =>
            {
                control.PointerPressed -= OnPointerPressed;
                control.PointerPressed += OnPointerPressed;
            });
        }

        private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Control control)
            {
                var item = GetItem(control);
                var gallery = control.FindAncestorOfType<Controls.ImageGallery>();
                if (gallery != null)
                {
                    gallery.SelectedItem = (ImageGalleryItem)item;
                }
            }
        }
    }
}
