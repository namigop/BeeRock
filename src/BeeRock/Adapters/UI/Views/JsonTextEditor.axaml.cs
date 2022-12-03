using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using BeeRock.Core.Utils;

namespace BeeRock.Views;

public partial class JsonTextEditor : UserControl {
    //private TextEditor editor;
    private CharFoldingStrategy folding;
    private FoldingManager foldingManager;
    private DispatcherTimer foldingTimer;

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<JsonTextEditor, string>(
            "Text",
            "",
            false,
            Avalonia.Data.BindingMode.TwoWay,
            null,
            OnCoerceText);

    private static string OnCoerceText(IAvaloniaObject d, string arg2) {
        var sender = d as JsonTextEditor;
        if (arg2 != sender.Editor.Text) {
            sender.Editor.Text = arg2;
            sender.foldingTimer.IsEnabled = true;

        }

        return arg2;
    }

    public JsonTextEditor() {
        InitializeComponent();
        //this.editor = this.Find<TextEditor>("editor");
        Editor.Document = new TextDocument();
        Editor.TextChanged += (sender, args) => Text = Editor.Text;
        folding = new CharFoldingStrategy('{', '}');
        foldingTimer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(2)
        };
        foldingTimer.Tick += FoldingTimer_Tick;
        foldingTimer.IsEnabled = false;
        SetupSyntaxHighlighting();
    }

    public string Text {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private void SetupSyntaxHighlighting() {
        // Load our custom highlighting definition
        using (var resource =
               typeof(TextEditor).Assembly.GetManifestResourceStream("AvaloniaEdit.Highlighting.Resources.Json.xshd")) {
            if (resource != null)
                using (var reader = new System.Xml.XmlTextReader(resource)) {
                    Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
        }
    }

    private void FoldingTimer_Tick(object sender, EventArgs e) {
        if (foldingManager == null)
            foldingManager = FoldingManager.Install(Editor.TextArea);

        if (foldingManager != null && Editor.Document.TextLength > 0)
            folding.UpdateFoldings(foldingManager, Editor.Document);
    }
}
