using System.ComponentModel;
using Avalonia.Controls;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI.Views;

public partial class ReqRespTraceWindow : Window {
    public ReqRespTraceWindow() {
        InitializeComponent();
        var vm = new ReqRespTraceViewModel(this);
        vm.Load();
        Closing += OnClosing;
        this.TraceControl.DataContext = vm;

    }

    private void OnClosing(object sender, CancelEventArgs e) {
        TraceControl.Close();
    }
}
