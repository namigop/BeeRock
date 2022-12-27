using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ViewModelBase : ReactiveObject, IDisposable {
    protected List<IDisposable> disposable = new();
    private bool disposedValue;

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                foreach (var d in disposable)
                    d?.Dispose();

                disposable.Clear();
            }

            disposedValue = true;
        }
    }
}