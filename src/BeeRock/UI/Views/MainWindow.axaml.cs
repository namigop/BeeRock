using System.ComponentModel;
using Avalonia.Controls;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        Closed += (sender, args) => ViewModel.Dispose();
    }

    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public void Init() {
        ViewModel.PropertyChanged += OnChanged;
        ViewModel.RequestClose += OnRequestClose;
        ViewModel.Init();
    }

    private void OnRequestClose(object sender, EventArgs e) {
        Close();
    }

    private void OnChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(MainWindowViewModel.HasService))
            if (ViewModel.HasService)
                WindowState = WindowState.Maximized;
    }
}
