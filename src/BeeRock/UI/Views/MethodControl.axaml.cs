using Avalonia.Controls;
using Avalonia.Interactivity;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI.Views;

public partial class MethodControl : UserControl {
    private Flyout _flyOut;

    public MethodControl() {
        InitializeComponent();
    }

    private void OnDeleteRuleClick_No(object sender, RoutedEventArgs e) {       
        _flyOut.Hide();
    }

    private void Flyout_OnOpened(object sender, EventArgs e) {
        _flyOut = (Flyout)sender;
    }

    private void OnDeleteRuleClick_Yes(object sender, RoutedEventArgs e) {
        var ruleItem = (RuleItem)((Button)sender).DataContext;
        var vm = (ServiceMethodItem)this.DataContext;
        vm.DeleteRuleCommand.Execute(ruleItem);
        _flyOut.Hide();
    }


}
