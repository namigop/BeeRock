using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace BeeRock.Core.Entities.Middlewares;

public static class DynamicRoutingMiddleware {
    public static void ConfigureDynamicRouting(this IApplicationBuilder app) {
        var handler = new DynamicRequestHandler();
        app.Use(async (context, next) => {
            context.Request.EnableBuffering();
            var bufferSize = 1024 * 50;
            string requestBody = "";
            // Leave the body open so the next middleware can read it.
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, bufferSize, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            var sourceUri = new Uri(context.Request.GetEncodedUrl());
            handler.Handle(context, requestBody, sourceUri);

            //the response will be a "passthrough" and will be handled by the PassThroughMiddleware
            await next(context);
        });
    }
}
