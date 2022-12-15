using System.ComponentModel;
using Avalonia.Controls;

namespace BeeRock.Adapters.UI.Views;

public partial class LogWindow : Window {
    public LogWindow() {
        InitializeComponent();
        Closing += OnClosing;
    }

    private void OnClosing(object sender, CancelEventArgs e) {
        LogControl.Close();
    }
}
