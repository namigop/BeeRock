using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BeeRock.Core.Entities;

public class ServerHostingService {
    private IWebHost _server;
    private string _serverStatus;

    public async Task RestartServer(RestServiceSettings settings, Type[] targetControllerTypes = null) {
        await StopServer(settings);

        if (!settings.Enabled) return;

        _server = WebHost.CreateDefaultBuilder().UseKestrel(serverOptions => {
                serverOptions.ListenAnyIP(settings.PortNumber);
                serverOptions.ListenLocalhost(settings.PortNumber);
            })
            .UseStartup(c => new ApiStartup(c.Configuration) { TargetControllers = targetControllerTypes })
            .UseDefaultServiceProvider((b, o) => { })
            .Build();

        _serverStatus = "Starting";
        _ = Task.Run(() => {
            _ = _server.RunAsync();
            _serverStatus = "Started";
        });
    }

    public async Task StopServer(RestServiceSettings settings) {
        if (_server != null) {
            _serverStatus = "Shutting down";
            await _server.StopAsync();
        }

        await Task.Delay(1000);
        _serverStatus = "Down";
    }

    public string GetServerStatus() {
        return _serverStatus;
    }
}
