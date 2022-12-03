using System.ComponentModel;
using Avalonia.Controls;
using BeeRock.APP.Services;
using BeeRock.ViewModels;

namespace BeeRock.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    public void Init() {
        ViewModel.PropertyChanged += OnChanged;
    }

    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    private void OnChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ViewModel.HasService))
            if (ViewModel.HasService)
                WindowState = WindowState.Maximized;
    }
}
