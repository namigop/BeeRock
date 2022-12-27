using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaEdit.TextMate;
using AvaloniaEdit.TextMate.Grammars;

namespace BeeRock.UI.Views;

public partial class LogControl : UserControl {
    private DispatcherTimer _timer;

    public LogControl() {
        InitializeComponent();

        SetupLogTimer();
        SetupSyntaxHighlighting();
    }

    private void SetupLogTimer() {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += OnTick;
        _timer.IsEnabled = true;
    }

    private void SetupSyntaxHighlighting() {
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var textMateInstallation = Editor.InstallTextMate(registryOptions);
        //textMateInstallation.SetGrammar(
        //    registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".txt").Id));
    }

    private void OnTick(object sender, EventArgs e) {
        var all = Global.Trace.Read();
        if (string.IsNullOrWhiteSpace(all))
            return;

        //Keep the number of lines of logs low in order to keep memory usage low
        if (Editor.LineCount > 500) {
            var half = Editor.LineCount / 2;
            var end = Editor.Document.Lines[half].EndOffset;
            Editor.Document.Remove(0, end);
        }

        using var reader = new StringReader(all);
        while (reader.ReadLine() is { } line)
            if (!line.Contains("Microsoft.AspNetCore."))
                Editor.AppendText($"{line}{Environment.NewLine}");

        Editor.ScrollToEnd();
    }

    public void Close() {
        Editor.Text = "";
        _timer.Stop();
    }
}
