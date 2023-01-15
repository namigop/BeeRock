using BeeRock.Core.Utils;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeeRock.Core.Entities;

public class ApiStartup {

    public ApiStartup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    public Type[] TargetControllers { get; set; }

    public void Configure(IApplicationBuilder app) {
        // app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.AllowOptionsForCORS();
        app.ConfigureExceptionHandler();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseHttpLogging();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        Requires.NotNullOrEmpty(services, nameof(services));
        Requires.NotNullOrEmpty(TargetControllers, nameof(TargetControllers));

        services.AddHttpLogging(l => { l.LoggingFields = HttpLoggingFields.All; });

        //start only the specific ontroller
        services.AddMvcCore().UseSpecificControllers(TargetControllers);
        services.AddControllers();
        services.AddSwaggerGen(opt => {
            
        });
       
    }
}