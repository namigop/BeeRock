using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Threading;
using BeeRock.APP.Services;
using BeeRock.ViewModels;

namespace BeeRock.Views;

public partial class LogControl :UserControl {
    private readonly DispatcherTimer timer;

    public LogControl() {
        InitializeComponent();
        this.timer = new DispatcherTimer();
        this.timer.Interval = TimeSpan.FromMilliseconds(500);
        this.timer.Tick += OnTick;
        this.timer.IsEnabled = true;
    }

    private void OnTick(object sender, EventArgs e) {
        string all = Global.Trace.Read();
        if (string.IsNullOrWhiteSpace(all))
            return;

        this.Editor.AppendText(all);
        this.Editor.ScrollToEnd();
    }
}
