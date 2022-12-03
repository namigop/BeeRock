using BeeRock.Core.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BeeRock.Adapters;

public class Settings {
    public bool Enabled { get; set; } = true;
    public int PortNumber { get; set; } = 7001;
}

public class ServerService {
    private IWebHost _server;

    private string _serverStatus;

    public void RestartServer(Settings settings, Type[] targetControllerTypes = null) {
        StopServer(settings);

        if (!settings.Enabled) return;

        _server = WebHost.CreateDefaultBuilder().UseKestrel(serverOptions => {
                serverOptions.ListenAnyIP(settings.PortNumber);
                serverOptions.ListenLocalhost(settings.PortNumber);
            })
            .UseStartup(c => new ApiStartup(c.Configuration)
                { TargetControllers = targetControllerTypes })
            .UseDefaultServiceProvider((b, o) => { })
            .Build();

        _serverStatus = "Starting";


        Task.Run(() => {
            Thread.Sleep(3000);
            _server.RunAsync();
            _serverStatus = "Started";
        });
    }

    public void StopServer(Settings settings) {
        if (_server != null) {
            _serverStatus = "Shutting down";

            _server.StopAsync().Wait();
        }

        Thread.Sleep(3000);
        _serverStatus = "Down";
    }

    public string GetServerStatus() {
        return _serverStatus;
    }
}