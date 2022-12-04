using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Entities;

namespace BeeRock;

public static class Global {
    public static ServiceItemCollection CurrentServices { get; set; }
    public static ConsoleIntercept Trace { get; set; }
}
