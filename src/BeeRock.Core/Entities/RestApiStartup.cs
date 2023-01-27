﻿using BeeRock.Core.Entities.Middlewares;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BeeRock.Core.Entities;

public class ApiStartup {
    public ApiStartup(IConfiguration configuration) {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }
    public Type[] TargetControllers { get; init; }
    public string ServiceName { get; init; }

    public void Configure(IApplicationBuilder app) {
        // app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.AllowOptionsForCORS();
        app.CheckForPassThroughResponses();
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

        //start only the specific controller
        services.AddMvcCore().UseSpecificControllers(TargetControllers);
        services.AddControllers();

        services.AddSwaggerGen(opt => {
            opt.SwaggerDoc("v1", new OpenApiInfo {
                Version = "v1",
                Title = ServiceName,
                Description = "Mock Service generated by BeeRock"
            });

            opt.SchemaFilter<XEnumNamesSchemaFilter>();
        });
    }
}

public class XEnumNamesSchemaFilter : ISchemaFilter {
    private const string NAME = "x-enumNames";

    public void Apply(OpenApiSchema model, SchemaFilterContext context) {
        var typeInfo = context.Type;
        // Chances are something in the pipeline might generate this automatically at some point in the future
        // therefore it's best to check if it exists.
        if (typeInfo.IsEnum && !model.Extensions.ContainsKey(NAME)) {
            var names = Enum.GetNames(context.Type);
            var arr = new OpenApiArray();
            arr.AddRange(names.Select(name => new OpenApiString(name)));
            model.Extensions.Add(NAME, arr);
        }
    }
}
