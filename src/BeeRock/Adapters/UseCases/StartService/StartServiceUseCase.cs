using BeeRock.Core.Entities;
using BeeRock.Ports;
using LanguageExt;

namespace BeeRock.Adapters.UseCases.StartService;

public class StartServiceUseCase : UseCaseBase, IStartServiceUseCase {
    public TryAsync<bool> Start(RestService service) {
        return async () => {
            Info("Starting server...");
            await Task.Run(() => {
                var svc = new ServerService();
                svc.RestartServer(service.Settings, service.ControllerTypes);
            });

            Info($"Server is up and running at port {service.Settings.PortNumber}");
            return true;
        };
    }
}