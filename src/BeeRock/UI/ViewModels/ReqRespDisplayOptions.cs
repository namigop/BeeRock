using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ReqRespDisplayOptions : ReactiveObject {
    private bool _canShowPost = true;
    private bool _canShowPut = true;
    private bool _canShowDelete = true;
    private bool _canShowHead;
    private bool _canShowOptions;
    private bool _canShowPatch;
    private bool _canShowGet = true;


    public bool CanShowGet {
        get => _canShowGet;
        set => this.RaiseAndSetIfChanged(ref _canShowGet, value);
    }

    public bool CanShowPost {
        get => _canShowPost;
        set => this.RaiseAndSetIfChanged(ref _canShowPost, value);
    }

    public bool CanShowPut {
        get => _canShowPut;
        set => this.RaiseAndSetIfChanged(ref _canShowPut, value);
    }

    public bool CanShowDelete {
        get => _canShowDelete;
        set => this.RaiseAndSetIfChanged(ref _canShowDelete, value);
    }

    public bool CanShowHead {
        get => _canShowHead;
        set => this.RaiseAndSetIfChanged(ref _canShowHead, value);
    }

    public bool CanShowOptions {
        get => _canShowOptions;
        set => this.RaiseAndSetIfChanged(ref _canShowOptions, value);
    }

    public bool CanShowPatch {
        get => _canShowPatch;
        set => this.RaiseAndSetIfChanged(ref _canShowPatch, value);
    }
}
