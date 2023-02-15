using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using AvaloniaEdit.Folding;
using AvaloniaEdit.TextMate;
using AvaloniaEdit.TextMate.Grammars;

namespace BeeRock.UI.Views;

public partial class JsonTextEditor : UserControl {
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<JsonTextEditor, string>(
            "Text",
            "",
            false,
            BindingMode.TwoWay,
            null,
            OnCoerceText);

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<JsonTextEditor, bool>(
            "IsReadOnly",
            false,
            false,
            BindingMode.TwoWay,
            null,
            OnCoerceIsReadOnly);


    //private TextEditor editor;
    private readonly CharFoldingStrategy _folding;
    private readonly DispatcherTimer _foldingTimer;
    private FoldingManager _foldingManager;
    TextMate.Installation _textMateInstallation;

    public JsonTextEditor() {
        InitializeComponent();
        Editor.Document = new TextDocument { Text = "" };
        Editor.TextChanged += OnTextChanged; // (_, _) => Text = Editor.Text;
        _folding = new CharFoldingStrategy('{', '}');
        _foldingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _foldingTimer.Tick += FoldingTimer_Tick;
        _foldingTimer.IsEnabled = false;

        Editor.Options.EnableHyperlinks = false;
        
        this.DetachedFromVisualTree += this.JsonTextEditor_DetachedFromVisualTree;
        SetupSyntaxHighlighting();
    }

    private void JsonTextEditor_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e) {
        var t = sender as JsonTextEditor;
        t._textMateInstallation?.Dispose();
        t._textMateInstallation = null;
        t._foldingTimer.Stop();
        t._foldingTimer.Tick -= FoldingTimer_Tick;
        t.Editor.TextChanged -= OnTextChanged;

    }
 

    public string Text {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsReadOnly {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    private static bool OnCoerceIsReadOnly(IAvaloniaObject d, bool arg2) {
        var sender = (JsonTextEditor)d;
        sender.Editor.IsReadOnly = arg2;
        return arg2;
    }

    private void SetupSyntaxHighlighting() {
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        this._textMateInstallation = Editor.InstallTextMate(registryOptions);
        _textMateInstallation.SetGrammar(
            registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".js").Id));
    }

    private static string OnCoerceText(IAvaloniaObject d, string arg2) {
        var sender = (JsonTextEditor)d;
        if (arg2 != sender.Editor.Text) {
            sender.Editor.Text = arg2;
            sender._foldingTimer.IsEnabled = true;
        }

        return arg2;
    }

    // private void SetupSyntaxHighlighting() {
    //     // Load our custom highlighting definition
    //     using (var resource =
    //            typeof(TextEditor).Assembly.GetManifestResourceStream("AvaloniaEdit.Highlighting.Resources.Json.xshd")) {
    //         if (resource != null)
    //             using (var reader = new XmlTextReader(resource)) {
    //                 Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
    //             }
    //     }
    // }

    private void FoldingTimer_Tick(object sender, EventArgs e) {
        if (_foldingManager == null)
            _foldingManager = FoldingManager.Install(Editor.TextArea);

        if (_foldingManager != null && Editor.Document.TextLength > 0)
            _folding.UpdateFoldings(_foldingManager, Editor.Document);
    }
    void OnTextChanged(object sender, EventArgs e) {
        this.Text = Editor.Text;
    }
}
