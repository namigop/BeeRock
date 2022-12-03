using Avalonia.Controls;
using Avalonia.Threading;

namespace BeeRock.Adapters.UI.Views;

public partial class LogControl : UserControl {
    private readonly DispatcherTimer _timer;

    public LogControl() {
        InitializeComponent();
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += OnTick;
        _timer.IsEnabled = true;
    }

    private void OnTick(object sender, EventArgs e) {
        var all = Global.Trace.Read();
        if (string.IsNullOrWhiteSpace(all))
            return;

        Editor.AppendText(all);
        Editor.ScrollToEnd();
    }
}