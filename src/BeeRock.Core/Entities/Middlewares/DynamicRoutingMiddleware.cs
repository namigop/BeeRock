using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;

namespace BeeRock.Core.Entities.Middlewares;

public static class DynamicRoutingMiddleware {
    public static void ConfigureDynamicRouting(this IApplicationBuilder app) {
        var handler = new DynamicRequestHandler();
        app.Use(async (context, next) => {
            var sourceUri = new Uri(context.Request.GetEncodedUrl());
            handler.Handle(context, sourceUri);

            //the response will be a "passthrough" and will be handled by the PassThroughMiddleware
            await next(context);
        });
    }
}
