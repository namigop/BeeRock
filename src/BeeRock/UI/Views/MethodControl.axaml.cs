using Avalonia.Controls;
using Avalonia.Interactivity;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI.Views;

public partial class MethodControl : UserControl {
    private Flyout _flyOut;

    public MethodControl() {
        InitializeComponent();
    }

    private void OnDeleteClick_No(object sender, RoutedEventArgs e) {
        //var vm = (ServiceManagementItem)((Button)sender).DataContext;
        //vm.DeleteCommand.Execute(false);
        _flyOut.Hide();
    }

    private void Flyout_OnOpened(object sender, EventArgs e) {
        _flyOut = (Flyout)sender;
    }

    private void OnDeleteClick_Yes(object sender, RoutedEventArgs e) {
        var vm = (ServiceMethodItem)((Button)sender).DataContext;
        vm.CreateNewRuleCommand.Execute(null);
        _flyOut.Hide();
    }


}
