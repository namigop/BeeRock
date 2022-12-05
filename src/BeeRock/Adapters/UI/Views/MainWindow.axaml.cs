using System.ComponentModel;
using Avalonia.Controls;
using BeeRock.Adapters.UI.ViewModels;

namespace BeeRock.Adapters.UI.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public void Init() {
        ViewModel.PropertyChanged += OnChanged;
        ViewModel.RequestClose += OnRequestClose;
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