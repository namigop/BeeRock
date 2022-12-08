using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Entities;
using BeeRock.Core.Utils;

namespace BeeRock;

public static class Global {
    public static ServiceItemCollection CurrentServices { get; set; }
    public static ConsoleIntercept Trace { get; set; }

    public static string LocalAppDataPath {
        get => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            .Then(p => Path.Combine(p, "BeeRock"));
    }
}
