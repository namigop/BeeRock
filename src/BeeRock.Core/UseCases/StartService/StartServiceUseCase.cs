using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.StartService;

public class StartServiceUseCase : UseCaseBase, IStartServiceUseCase {
    public TryAsync<IServerHostingService> Start(IRestService service) {
        return async () => {
            Info("Starting server...");

            var svc = new ServerHostingService(service.Name, service.Settings, service.ControllerTypes);
            await svc.StartServer();
            Info($"Server is up and running at port {service.Settings.PortNumber}");
            return svc;
        };
    }
}
