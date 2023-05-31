using System.Net;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.Tracing;
using BeeRock.Repository;

namespace BeeRock;

public static class Startup {
    public static void Start() {
        Global.Trace = new ConsoleIntercept();
        Console.SetOut(Global.Trace);
        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
        var tracer = ReqRespTracer.Instance.Value;
        tracer.Setup(new DocReqRespTraceRepo(Db.GetTraceDb()));
    }
}
