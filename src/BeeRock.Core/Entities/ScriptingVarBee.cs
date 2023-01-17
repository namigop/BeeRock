namespace BeeRock.Core.Entities;

public class ScriptingVarBee {
    public const string VarName = "bee";
    public ScriptingVarFileResponse FileResp { get; } = new();
    public ScriptingVarRun Run { get; } = new();

    public ScriptingVarRmq Rmq { get; } = new();
}
