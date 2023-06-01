using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using BeeRock.Core.Dtos;
using BeeRock.Core.Entities.Middlewares;
using BeeRock.Core.Entities.Tracing;
using BeeRock.Core.Utils;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ReqRespTraceViewModel : ViewModelBase {
    private readonly Window _window;
    private ReqRespTraceItem _selectedTraceItem;
    public ObservableCollection<ReqRespTraceItem> TraceItems { get; } = new();

    //for the designer
    public ReqRespTraceViewModel() :this(null)
    {}
    public ReqRespTraceViewModel(Window window) {
        _window = window;
        this.ClearTracesCommand = ReactiveCommand.Create(OnClear);
        this.SaveTraceCommand = ReactiveCommand.Create(OnSave);
        this.DisplayOptions = new ReqRespDisplayOptions();

        this.WhenAnyValue(
                t => t.DisplayOptions.CanShowGet,
                t => t.DisplayOptions.CanShowPost,
                t => t.DisplayOptions.CanShowPut,
                t => t.DisplayOptions.CanShowDelete,
                t => t.DisplayOptions.CanShowPatch,
                t => t.DisplayOptions.CanShowOptions,
                t => t.DisplayOptions.CanShowHead
            )
            .Throttle(TimeSpan.FromMilliseconds(250))
            .Subscribe(t => Load())
            .Void(d => disposable.Add(d));
    }

    public ReqRespDisplayOptions DisplayOptions { get; }

    public ReqRespTraceItem SelectedTraceItem {
        get => _selectedTraceItem;
        set {
            this.RaiseAndSetIfChanged(ref _selectedTraceItem, value);
            _selectedTraceItem?.PrettyPrint();
        }
    }

    public ICommand SaveTraceCommand { get; }
    public ICommand ClearTracesCommand { get; }

    private void OnClear() {
        this.TraceItems.Clear();
        ReqRespTracer.Instance.Value.ClearAll();
    }
    private async Task OnSave() {
        if (this.SelectedTraceItem != null) {
            var json = this.SelectedTraceItem.ToJson();
            var dg = new SaveFileDialog {
                DefaultExtension = ".json",
                InitialFileName = "trace.json"
            };
            var file = await dg.ShowAsync(_window);
            File.WriteAllText(file, json);

        }
    }

    public bool CheckIfCanShow(DocReqRespTraceDto i) {
        if (DisplayOptions.CanShowGet && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Get) {
            return true;
        }

        if (DisplayOptions.CanShowPost && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Post) {
            return true;
        }

        if (DisplayOptions.CanShowPut && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Put) {
            return true;
        }

        if (DisplayOptions.CanShowDelete && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Delete) {
            return true;
        }

        if (DisplayOptions.CanShowOptions && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Options) {
            return true;
        }

        if (DisplayOptions.CanShowPatch && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Patch) {
            return true;
        }

        if (DisplayOptions.CanShowHead && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Head) {
            return true;
        }


        return false;
    }

    public void Load() {
        var all = ReqRespTracer.Instance.Value.GetAll().Where(CheckIfCanShow);
        this.TraceItems.Clear();
        foreach (var i in all) {
            var item = new ReqRespTraceItem(i);
            this.TraceItems.Add(item);
        }

        if (this.TraceItems.Any())
            this.SelectedTraceItem = this.TraceItems.First();
    }
}
