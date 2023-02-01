using BeeRock.Core.Utils;

// ReSharper disable UnusedMember.Global

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingLog {
    public void Info(string msg) {
        C.Info($"[script] : {msg}");
    }

    public void Error(string msg) {
        C.Error($"[script] : {msg}");
    }

    public void Warn(string msg) {
        C.Warn($"[script] : {msg}");
    }
}
