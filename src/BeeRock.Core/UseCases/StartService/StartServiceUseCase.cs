using BeeRock.Core.Entities.DynamicRouting;
using BeeRock.Core.Entities.Hosting;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.StartService;

public class StartServiceUseCase : UseCaseBase, IStartServiceUseCase {
    /// <summary>
    ///     Start hosting the generated rest service
    /// </summary>
    public TryAsync<IServerHostingService> Start(ICompiledRestService service) {
        return async () => {
            C.Info("Starting server...");

            var name = service.Name.ToLower().Contains("mock") ? service.Name : $"(mock) {service.Name}";
            var startup = new RestApiStartup { TargetControllers = service.ControllerTypes, ServiceName = name };
            var svc = new ServerHostingService(startup, service.Name, service.Settings);
            await svc.StartServer();
            Info($"Server is up and running at port {service.Settings.PortNumber}");
            return svc;
        };
    }

    /// <summary>
    ///     Start hosting the generated rest service
    /// </summary>
    public TryAsync<IServerHostingService> Start(IRestService service) {
        if (service is ICompiledRestService c)
            return Start(c);

        return async () => {
            C.Info("Starting dynamic server...");
            var name = service.Name.ToLower().Contains("mock") ? service.Name : $"(mock) {service.Name}";
            var startup = new DynamicRoutingStartup(name);
            var svc = new ServerHostingService(startup, service.Name, service.Settings);
            await svc.StartServer();
            Info($"Server is up and running at port {service.Settings.PortNumber}");
            return svc;
        };
    }
}
