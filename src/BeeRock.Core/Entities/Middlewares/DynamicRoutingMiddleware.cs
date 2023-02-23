using System.Diagnostics;
using System.Net;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace BeeRock.Core.Entities.Middlewares;

public static class DynamicRoutingMiddleware {


    public static void Configure(this IApplicationBuilder app, IProxyRouteHandler proxyRouteHandler) {
        app.Use(async (context, next) => {
            var sourceUri = new Uri(context.Request.GetEncodedUrl());
            var targetUri = proxyRouteHandler.Selector.BuildUri(sourceUri); //   BuildTargetUri(context.Request, "https://scl-apigateway.cxos.tech");
            if (targetUri != null) {
                C.Info($"Routing HTTP {context.Request.Method} to {targetUri}");

                //get the list of dynamic services
                //var resp = RequestHandler.Handle("TODO", CreateScriptVariables())
                //handle RestException


                return;
            }

            await next(context);
        });
    }

    private static void CopyResponseHeaders(HttpContext context, HttpResponseMessage responseMessage) {
        foreach (var header in responseMessage.Headers) context.Response.Headers[header.Key] = header.Value.ToArray();

        foreach (var header in responseMessage.Content.Headers) context.Response.Headers[header.Key] = header.Value.ToArray();

        context.Response.Headers.Remove("transfer-encoding");
    }

}
