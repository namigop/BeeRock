using Autofac;
using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Entities;
using BeeRock.Core.Utils;

namespace BeeRock;

public static class Global {
    public static ServiceItemCollection CurrentServices { get; set; }
    public static ConsoleIntercept Trace { get; set; }

    public static string AppDataPath =>
        Environment.GetFolderPath(Environment.SpecialFolder.Favorites)
            .Then(p => Path.Combine(p, "..", "BeeRock"));

    public static IContainer Resolver { get; set; }

    public static string DbFile => Path.Combine(AppDataPath, "BeeRock.db");
    public static string TempPath => Path.Combine(AppDataPath, "Temp");
}
