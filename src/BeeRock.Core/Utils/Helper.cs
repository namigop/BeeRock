#nullable enable
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using BeeRock.Core.Entities.Hosting;
using Newtonsoft.Json;

namespace BeeRock.Core.Utils;

public static class Helper {
    public static string GetAppDataPath() {
        if (IsWindows())
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                .Then(p => Path.Combine(p, "BeeRock"))
                .Then(p => Directory.CreateDirectory(p).FullName);

        // if (IsMacOs()) {
        //     return Environment.GetFolderPath(Environment.SpecialFolder.Favorites)
        //         .Then(p => Path.Combine(p, "..", "BeeRock"));
        // }

        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            .Then(p => Path.Combine(p, "BeeRock"))
            .Then(p => Directory.CreateDirectory(p).FullName);
    }

    public static string GetTempPath() {
        return Path.Combine(GetAppDataPath(), "Temp");
    }

    public static bool IsLinux() {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static RestHttpException? FindRestHttpException(Exception? exc) {
        if (exc == null)
            return null;

        if (exc is RestHttpException r)
            return r;

        return FindRestHttpException(exc?.InnerException);
    }

    public static bool IsMacOs() {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }

    public static bool IsWindows() {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    public static void OpenBrowser(string url) {
        // hack because of this: https://github.com/dotnet/corefx/issues/10361
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //url = url.Replace("&", "^&");
            //Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Process.Start("xdg-open", url);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Process.Start("open", url);
        else
            throw new PlatformNotSupportedException($"Unable to open URL: {url}");
    }

    public static string RemoveComments(string text) {
        using var reader = new StringReader(text);
        var sb = new StringBuilder();
        while (reader.ReadLine() is { } line)
            if (!line.TrimStart().StartsWith("//"))
                sb.AppendLine(line);

        return sb.ToString();
    }

    public static T? Deserialize<T>(string json) {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static string Serialize<T>(T inst) {
        return JsonConvert.SerializeObject(inst);
    }

    public static object? Deserialize(string value, Type type) {
        return JsonConvert.DeserializeObject(value, type);
    }

    public static string Serialize(object? n, Type t) {
        return JsonConvert.SerializeObject(n, t, Formatting.None, new JsonSerializerSettings());
    }
}
