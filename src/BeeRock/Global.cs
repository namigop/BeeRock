using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Entities;
using BeeRock.Core.Utils;

namespace BeeRock;

public static class Global {
    public static TabItemCollection CurrentServices { get; set; }
    public static ConsoleIntercept Trace { get; set; }

    public static string AppDataPath { get; } = Helper.GetAppDataPath();

    public static string DbFile => Path.Combine(AppDataPath, "BeeRock.db");

    public static string TempPath => Path.Combine(AppDataPath, "Temp");
}
