using System.Diagnostics.CodeAnalysis;
using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingLog {
    public void Info(string msg) {
        C.Info(msg);
    }
    public void Error(string msg) {
        C.Error(msg);
    }
    public void Warn(string msg) {
        C.Warn(msg);
    }
}
