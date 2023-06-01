using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BeeRock.Core.Entities.Middlewares;

public static class DynamicRoutingMiddleware {
    public static void ConfigureDynamicRouting(this IApplicationBuilder app) {
        var handler = new DynamicRequestHandler();
        app.Use(async (context, next) => {
            var method = HttpUtil.GetMethod(context.Request.Method);
            if ( method == HttpMethod.Get ||
                 method == HttpMethod.Post ||
                 method == HttpMethod.Put ||
                 method == HttpMethod.Delete ||
                 method == HttpMethod.Patch) {
                var requestBody = await HttpUtil.BufferRequestBody(context.Request);
                var sourceUri = new Uri(context.Request.GetEncodedUrl());
                handler.Handle(context, requestBody, sourceUri);
            }

            //the response will be a "passthrough" and will be handled by the PassThroughMiddleware
            await next(context);
        });
    }
}
