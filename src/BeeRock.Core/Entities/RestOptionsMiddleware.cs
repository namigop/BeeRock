using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities;

public static class RestOptionsMiddleware {
    public static void AllowOptionsForCORS(this IApplicationBuilder app) {
        app.Use(async (context, next) => {
            var methodvalue = context.Request.Method;
            if (!string.IsNullOrEmpty(methodvalue)) {
                if (methodvalue == HttpMethods.Options || methodvalue == HttpMethods.Head) {
                    await context.Response.CompleteAsync();
                }
                else {
                    await next();
                }
            }
        });
    }
}