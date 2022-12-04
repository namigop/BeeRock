using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using BeeRock.Core.Entities;

namespace BeeRock.Ports;

public abstract class UseCaseBase : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<Log> LogEvent;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
        if (!EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }


    public IDisposable AddWatch(Action<string> func) {
        return Observable.FromEventPattern<Log>(this, nameof(LogEvent))
            .Subscribe(arg => { func(arg.EventArgs.Message); });
    }

    protected void Info(string msg) {
        LogEvent?.Invoke(this, new Log(LogType.Info, msg));
    }

    protected void Error(string msg) {
        LogEvent?.Invoke(this, new Log(LogType.Error, msg));
    }

    protected void Warn(string msg) {
        LogEvent?.Invoke(this, new Log(LogType.Warning, msg));
    }

    [Conditional("DEBUG")]
    protected void Debug(string msg) {
        LogEvent?.Invoke(this, new Log(LogType.Debug, msg));
    }
}