using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace BeeRock.Core.Utils;

public class Helper {
    public static bool IsLinux() {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static bool IsMacOs() {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }

    public static bool IsWindows() {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    public static string GetAppDataPath() {
        if (IsWindows())
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                .Then(p => Path.Combine(p, "BeeRock"));

        //osx and linux
        return Environment.GetFolderPath(Environment.SpecialFolder.Favorites)
            .Then(p => Path.Combine(p, "..", "BeeRock"));
    }

    public static string RemoveComments(string text) {
        using var reader = new StringReader(text);
        var sb = new StringBuilder();
        while (reader.ReadLine() is { } line)
            if (!line.TrimStart().StartsWith("//"))
                sb.AppendLine(line);

        return sb.ToString();
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
}
