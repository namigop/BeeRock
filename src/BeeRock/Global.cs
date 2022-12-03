using System.Diagnostics;
using BeeRock.Core.Entities;
using BeeRock.Models;

namespace BeeRock;

public static class Global {
    public static ServiceItem CurrentService { get; set; }
    public static ConsoleIntercept Trace { get; set; }
}
