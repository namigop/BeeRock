using BeeRock.API;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BeeRock.APP.Services;

public class Settings {
    public bool Enabled { get; set; } = true;
    public int PortNumber { get; set; } = 7001;
}

public class ServerService {
    private IWebHost server;

    private string serverStatus;

    public void RestartServer(Settings settings, Type[] targetControllerTypes = null) {
        StopServer(settings);

        if (!settings.Enabled) return;

        server = WebHost.CreateDefaultBuilder().UseKestrel(serverOptions => {
                serverOptions.ListenAnyIP(settings.PortNumber);
                serverOptions.ListenLocalhost(settings.PortNumber);
            })
            .UseStartup(c => new ApiStartup(c.Configuration)
                { TargetControllers = targetControllerTypes  })
            .UseDefaultServiceProvider((b, o) => { })
            .Build();

        serverStatus = "Starting";


        Task.Run(() => {
            Thread.Sleep(3000);
            server.RunAsync();
            serverStatus = "Started";
        });
    }

    public void StopServer(Settings settings) {
        if (server != null) {
            serverStatus = "Shutting down";

            server.StopAsync().Wait();
        }

        Thread.Sleep(3000);
        serverStatus = "Down";
    }

    public string GetServerStatus() {
        return serverStatus;
    }
}
