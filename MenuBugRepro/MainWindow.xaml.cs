using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MenuBugRepro.Interface;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;
using Window = Avalonia.Controls.Window;

namespace MenuBugRepro
{
    public sealed class MainWindow : Window
    {
        private readonly CustomTitleBar _titleBar;

        private static MainWindow? _instance;
        public static MainWindow Instance => _instance ??= new MainWindow();

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            this.AttachDevTools();

            // Weird OSX hack to hide decorations properly
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                SystemDecorations = SystemDecorations.Full;
                HasSystemDecorations = false;
            }

            _titleBar = this.FindControl<CustomTitleBar>("TitleBar");
            _titleBar.PointerPressed += (i, e) => PlatformImpl?.BeginMoveDrag(e);
            _titleBar.OptionsPressed += (i, e) =>
            {
                _titleBar.OptionsButton.ContextMenu?.Open(_titleBar.OptionsButton);
            };
            _titleBar.ClosePressed += (sender, args) => Close();

            BuildOptionsMenu();
        }

        private void BuildOptionsMenu()
        {
            var widget = this.FindControl<TextBlock>("TextContent");
            var options = new Dictionary<string, EventHandler<RoutedEventArgs>?>()
            {
                ["Menu item 1"] =
                    (sender, args) => widget.Text = "Item 1 selected",
                ["Menu item 2"] =
                    (sender, args) => widget.Text = "Item 2 selected"
            };

            _titleBar.OptionsButton.ContextMenu = MenuFactory.BuildContextMenu(options, _titleBar.OptionsButton);
        }
    }
}