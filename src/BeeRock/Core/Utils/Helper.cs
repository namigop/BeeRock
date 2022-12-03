using System.Diagnostics;
using System.Runtime.InteropServices;

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