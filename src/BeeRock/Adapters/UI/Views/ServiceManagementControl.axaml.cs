using Avalonia.Controls;
using Avalonia.Interactivity;
using BeeRock.Adapters.UI.ViewModels;

namespace BeeRock.Adapters.UI.Views;

public partial class ServiceManagementControl : UserControl {
    private Flyout _flyOut;

    public ServiceManagementControl() {
        InitializeComponent();
    }

    private void OnDeleteClick_No(object sender, RoutedEventArgs e) {
        var vm = (ServiceManagementItem)((Button)sender).DataContext;
        vm.DeleteCommand.Execute(false);
        _flyOut.Hide();
    }

    private void Flyout_OnOpened(object sender, EventArgs e) {
        _flyOut = (Flyout)sender;
    }

    private void OnDeleteClick_Yes(object sender, RoutedEventArgs e) {
        var vm = (ServiceManagementItem)((Button)sender).DataContext;
        vm.DeleteCommand.Execute(true);
        _flyOut.Hide();
    }
}
