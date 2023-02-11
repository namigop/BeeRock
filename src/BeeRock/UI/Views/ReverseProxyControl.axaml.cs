using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BeeRock.UI.Views;

public partial class ReverseProxyControl : UserControl {
    public ReverseProxyControl() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}