using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Middlewares;

public static class RestOptionsMiddleware {
    /// <summary>
    ///     Automatically respond to pre-flight OPTIONS request and add the CORS headers
    /// </summary>
    /// <param name="app"></param>
    public static void AllowOptionsForCORS(this IApplicationBuilder app) {
        app.Use(async (context, next) => {
            var method = context.Request.Method;
            if (!string.IsNullOrEmpty(method)) {
                if (method == HttpMethods.Options) {
                    context.Response.Headers.AccessControlAllowOrigin = "*";
                    context.Response.Headers.AccessControlAllowMethods = "GET,POST,PUT,PATCH,DELETE,HEAD,OPTIONS";
                    context.Response.Headers.AccessControlAllowHeaders = "Content-Type, Origin, Accept,Authorization,Content-Length, X-Requested-With";

                    await context.Response.CompleteAsync();
                }
                else {
                    await next();
                }
            }
        });
    }
}
