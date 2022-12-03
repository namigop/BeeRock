using BeeRock.Adapters.UI.Models;
using BeeRock.Core.Entities;

namespace BeeRock;

public static class Global {
    public static ServiceItem CurrentService { get; set; }
    public static ConsoleIntercept Trace { get; set; }
}