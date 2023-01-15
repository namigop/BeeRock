using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BeeRock.Core.Entities;

public class ServerHostingService : IServerHostingService {
    private readonly string _serviceName;
    private readonly RestServiceSettings _settings;
    private readonly Type[] _targetControllerTypes;
    private IWebHost _server;
    private string _serverStatus;

    public ServerHostingService(string serviceName, RestServiceSettings settings, Type[] targetControllerTypes = null) {
        _serviceName = serviceName;
        _settings = settings;
        _targetControllerTypes = targetControllerTypes;
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
            await _server.StartAsync();
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
                .UseStartup(c => new ApiStartup(c.Configuration) { TargetControllers = _targetControllerTypes })
                .UseDefaultServiceProvider((b, o) => { })
                .Build();
    }
}