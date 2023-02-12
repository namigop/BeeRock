using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI.Views;

public partial class ReverseProxyControl : UserControl {
    private Flyout _flyOut;
    public ReverseProxyControl() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void Flyout_OnOpened(object sender, EventArgs e) {
        _flyOut = (Flyout)sender;
    }

    private void OnDeleteClick_Yes(object sender, RoutedEventArgs e) {
        var vm = (TabItemReverseProxy)((Button)sender).DataContext;
        vm.SelectedProxyRoute?.DeleteCommand.Execute(true);
        _flyOut.Hide();
    }

    private void OnDeleteClick_No(object sender, RoutedEventArgs e) {
        var vm = (TabItemReverseProxy)((Button)sender).DataContext;
        vm.SelectedProxyRoute?.DeleteCommand.Execute(false);
        _flyOut.Hide();
    }
}
