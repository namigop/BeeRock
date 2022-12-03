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

    public Type[] TargetControllers { get; set; }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        services.AddHttpLogging(l => { l.LoggingFields = HttpLoggingFields.All; });
        services.AddMvcCore().UseSpecificControllers(TargetControllers);
        services.AddControllers();
        services.AddSwaggerGen();
    }


    public void Configure(IApplicationBuilder app) {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseHttpLogging();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
