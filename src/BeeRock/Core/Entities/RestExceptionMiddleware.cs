using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities;

public class RestHttpException : Exception {
    public HttpStatusCode StatusCode { get; init; }
    public string Error { get; init; }
}

public static class ExceptionMiddlewareExtensions {
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
                    }
                }
            });
        });
    }
}