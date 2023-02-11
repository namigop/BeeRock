using System.Net;
using BeeRock.Core.Entities;

namespace BeeRock;

public static class Startup {
    public static void Start() {
        Global.Trace = new ConsoleIntercept();
        Console.SetOut(Global.Trace);
        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
    }
}