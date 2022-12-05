using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaEdit.TextMate;
using AvaloniaEdit.TextMate.Grammars;
using Microsoft.CodeAnalysis.Differencing;

namespace BeeRock.Adapters.UI.Views;

public partial class LogControl : UserControl {
    private readonly DispatcherTimer _timer;

    public LogControl() {
        InitializeComponent();
        Global.Trace.Enabled = true;
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += OnTick;
        _timer.IsEnabled = true;

        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var textMateInstallation = Editor.InstallTextMate(registryOptions);
        textMateInstallation.SetGrammar(
            registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".js").Id));
    }

    private void OnTick(object sender, EventArgs e) {
        var all = Global.Trace.Read();
        if (string.IsNullOrWhiteSpace(all))
            return;

        using var reader = new StringReader(all);
        while (reader.ReadLine() is { } line) {
            if (!line.Contains("Microsoft.AspNetCore."))
                Editor.AppendText($"{line}{Environment.NewLine}");
        }

        Editor.ScrollToEnd();
    }
}
