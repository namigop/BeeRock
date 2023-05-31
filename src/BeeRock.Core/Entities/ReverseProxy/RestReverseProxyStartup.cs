using BeeRock.Core.Entities.Middlewares;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeeRock.Core.Entities.ReverseProxy;

public class RestReverseProxyStartup : IStartup {
    private readonly IProxyRouteHandler _proxyRouteHandler;

    public RestReverseProxyStartup(IProxyRouteHandler proxyRouteSelector) {
        _proxyRouteHandler = proxyRouteSelector;
    }

    public IConfiguration Configuration { get; private set; }

    public IStartup Setup(IConfiguration configuration) {
        Configuration = configuration;
        return this;
    }

    public string ServiceName { get; } = "BeeRock Reverse Proxy";

    public void Configure(IApplicationBuilder app) {
        app.ConfigureReqRespTracing();
        app.ConfigureReverseProxy(_proxyRouteHandler);
        app.UseRouting();
        app.UseCors();
        app.UseHttpLogging();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        services.AddCors(options => {
            options.AddDefaultPolicy(policy => {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();
                policy.SetIsOriginAllowed(o => true); //allows all origins everything
            });
        });

        services.AddHttpLogging(l => { l.LoggingFields = HttpLoggingFields.All; });
        services.AddMvcCore().UseSpecificControllers(typeof(ReverseProxyDummyController));
        services.AddControllers();
    }
}
