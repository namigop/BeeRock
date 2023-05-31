using Avalonia.Controls;
using Avalonia.Threading;
using BeeRock.Core.Dtos;
using BeeRock.Core.Entities.Tracing;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI.Views;

public partial class ReqRespTraceControl : UserControl {
    
    public ReqRespTraceControl() {
        InitializeComponent();
        ReqRespTracer.Instance.Value.Traced += OnTraced;
    }

    private void OnTraced(object sender, DocReqRespTraceDto e) {
        Dispatcher.UIThread.Post(() => {
            var vm = (ReqRespTraceViewModel)this.DataContext;
            if (vm.CheckIfCanShow(e)) {
                vm.TraceItems.Add(new ReqRespTraceItem(e));
            }
        });
    }

    public void Close() {
        ReqRespTracer.Instance.Value.Traced -= OnTraced;
    }
}
