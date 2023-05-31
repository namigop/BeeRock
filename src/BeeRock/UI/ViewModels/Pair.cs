using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class Pair<K,V> : ReactiveObject {
    private K _key;
    private V _value;

    public K Key {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key , value);
    }

    public V Value {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value , value);
    }
}