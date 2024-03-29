using BeeRock.Core.Entities.Middlewares;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeeRock.Core.Entities.DynamicRouting;

public class DynamicRoutingStartup : IStartup {
    public DynamicRoutingStartup(string serviceName) {
        ServiceName = serviceName;
    }

    public IConfiguration Configuration { get; private set; }

    public IStartup Setup(IConfiguration configuration) {
        Configuration = configuration;
        return this;
    }

    public string ServiceName { get; }

    public void Configure(IApplicationBuilder app) {
        app.ConfigureReqRespTracing();
        app.CheckForPassThroughResponses();
        app.ConfigureExceptionHandler();
        app.ConfigureDynamicRouting();
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
        services.AddMvcCore().UseSpecificControllers(typeof(DynamicRoutingDummyController));
        services.AddControllers();
    }
}
