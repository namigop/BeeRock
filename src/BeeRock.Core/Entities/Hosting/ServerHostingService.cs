using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BeeRock.Core.Entities.Hosting;

public class ServerHostingService : IServerHostingService {
    private readonly string _serviceName;
    private readonly RestServiceSettings _settings;
    private readonly IStartup _startup;

    private IWebHost _server;
    private string _serverStatus;

    public ServerHostingService(IStartup startup, string serviceName, RestServiceSettings settings) {
        _startup = startup;
        _serviceName = serviceName;
        _settings = settings;

        CanStart = true;
    }

    public bool CanStart { get; private set; }

    public bool CanStop => !CanStart;

    public string GetServerStatus() {
        return $"{_serviceName}:{_settings.PortNumber} {_serverStatus}";
    }

    /// <summary>
    ///     Start the web server
    /// </summary>
    public async Task StartServer() {
        await StopServer();

        if (!_settings.Enabled) return;

        TryCreateWebHost();
        if (CanStart) {
            _serverStatus = "Starting";
            _ = _server.StartAsync();
            _serverStatus = "Started";
            CanStart = false;
        }

        C.Info(GetServerStatus());
    }

    /// <summary>
    ///     Stop the web server
    /// </summary>
    public async Task StopServer() {
        if (CanStop) {
            _serverStatus = "Shutting down";
            await _server.StopAsync();
            _serverStatus = "Stopped";
            _server?.Dispose();
            _server = null;
            CanStart = true;
        }
        else {
            _serverStatus = "Down";
        }

        C.Info(GetServerStatus());
    }

    private void TryCreateWebHost() {
        if (_server == null)
            _server = WebHost.CreateDefaultBuilder()
                .UseKestrel(serverOptions => {
                    serverOptions.ListenAnyIP(_settings.PortNumber);
                    serverOptions.ListenLocalhost(_settings.PortNumber);
                })
                .UseStartup(c => _startup.Setup(c.Configuration))
                .UseDefaultServiceProvider((b, o) => { })
                .Build();
    }
}
