using BeeRock.Core.Entities;
using BeeRock.Core.Entities.Hosting;
using BeeRock.Core.Entities.ReverseProxy;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.StartService;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.StartReverseProxy;

public class StartReverseProxyUseCase : UseCaseBase, IStartReverseProxyUseCase {
    /// <summary>
    ///     Start hosting the generated rest service
    /// </summary>
    public TryAsync<IServerHostingService> Start(RestServiceSettings settings) {
        return async () => {
            C.Info("Starting reverse proxy server...");

            var startup = new RestReverseProxyStartup();
            var hosting = new ServerHostingService(startup, "ReverseProxy", settings);
            await hosting.StartServer();

            Info($"ReverseProxy Server is up and running at port {settings.PortNumber}");
            return hosting;
        };
    }
}
