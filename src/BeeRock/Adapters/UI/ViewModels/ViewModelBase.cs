using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class ViewModelBase : ReactiveObject, IDisposable
{
    protected List<IDisposable> disposable = new List<IDisposable>();
    private bool disposedValue;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var d in disposable)
                    d?.Dispose();

                disposable.Clear();
            }

            disposedValue = true;
        }
    }
}