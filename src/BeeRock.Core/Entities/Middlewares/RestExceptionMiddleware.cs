using System.Net.Mime;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Middlewares;

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
                    var r = Helper.FindRestHttpException(contextFeature.Error);
                    if (r != null) {
                        context.Response.StatusCode = (int)r.StatusCode;
                        context.Response.ContentType = MediaTypeNames.Text.Plain;
                        await context.Response.WriteAsync(r.Error);
                        return;
                    }

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = MediaTypeNames.Text.Plain;
#if DEBUG
                    await context.Response.WriteAsync(contextFeature.Error.InnerException.ToString());
#endif
#if !DEBUG
                     await context.Response.WriteAsync(contextFeature.Error.InnerException.Message);
#endif
                }
            });
        });
    }
}
