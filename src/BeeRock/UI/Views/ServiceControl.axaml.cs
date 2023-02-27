using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace BeeRock.UI.Views;

public partial class ServiceControl : UserControl {
    public ServiceControl() {
        InitializeComponent();
    }

    private void OnDeleteClick_No(object sender, RoutedEventArgs e) {
        var btn = (Button)sender;
        var p = btn.FindLogicalAncestorOfType<Button>();
        p?.Flyout?.Hide();
    }
}
