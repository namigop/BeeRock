using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.StartServiceUseCase;
using LanguageExt;

namespace BeeRock.Adapters.UseCases.StartService;

public class StartServiceUseCase : UseCaseBase, IStartServiceUseCase {
    public TryAsync<bool> Start(IRestService service) {
        return async () => {
            Info("Starting server...");

            var svc = new ServerHostingService();
            await svc.RestartServer(service.Settings, service.ControllerTypes);
            Info($"Server is up and running at port {service.Settings.PortNumber}");
            return true;
        };
    }
}
