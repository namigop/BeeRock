using System.Net.Mime;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities;

public static class RestExceptionMiddleware {
    /// <summary>
    ///     Converts a RestException to a proper HTTP response message
    /// </summary>
    /// <param name="app"></param>
    public static void ConfigureExceptionHandler(this IApplicationBuilder app) {
        app.UseExceptionHandler(appError => {
            appError.Run(async context => {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null) {
                    var inner = contextFeature.Error.InnerException;
                    if (inner is RestHttpException r) {
                        context.Response.StatusCode = (int)r.StatusCode;
                        context.Response.ContentType = MediaTypeNames.Text.Plain;
                        await context.Response.WriteAsync(r.Error);
                        return;
                    }

                    if (inner is TargetInvocationException t && inner.InnerException is RestHttpException r2) {
                        context.Response.StatusCode = (int)r2.StatusCode;
                        context.Response.ContentType = MediaTypeNames.Text.Plain;
                        await context.Response.WriteAsync(r2.Error);
                    }
                }
            });
        });
    }
}
