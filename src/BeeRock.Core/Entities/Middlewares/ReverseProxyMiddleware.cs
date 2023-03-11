using System.Diagnostics;
using System.Net;

using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace BeeRock.Core.Entities.Middlewares;

public static class ReverseProxyMiddleware {
    private static readonly HttpClient _httpClient = new();

    public static void ConfigureReverseProxy(this IApplicationBuilder app, IProxyRouteHandler proxyRouteHandler) {
        // Stages for Reverse Porxy
        //  1. Get RoutHandler
        //  2. BuildTargetUri
        //  2. CreateHttpReqMessage to target uri
        //  3. Send request
        //  4. Prepare Response
        //  5. Metric

        // Stages for Dynamic Proxy
        //  1. GetRoutHandler
        //  2.
        //  3. Prepare Response
        //  5. Metric

        _ = app.Use(async (HttpContext context, RequestDelegate next) => {
            var sourceUri = new Uri(context.Request.GetEncodedUrl());
            var (routeConfig, routeParameters) = proxyRouteHandler.Selector.FindMatchingRoute(sourceUri);
            var targetUri = proxyRouteHandler.Selector.BuildUri(sourceUri, routeConfig, routeParameters);
            if (targetUri != null) {
                C.Info($"Routing HTTP {context.Request.Method} to {targetUri}");
                var routeIndex = routeConfig.Index;
                proxyRouteHandler.Begin(routeConfig);

                var targetRequestMessage = HttpUtil.CreateRequestMessage(context.Request, targetUri);
                var sw = Stopwatch.StartNew();
                using var responseMessage = await Forward(targetRequestMessage,context.RequestAborted);
                sw.Stop();

                context.Response.StatusCode = (int)responseMessage.StatusCode;
                HttpUtil.CopyResponseHeaders(context.Response, responseMessage);

                var doNotWriteToBody = responseMessage.StatusCode == HttpStatusCode.NoContent ||
                                       targetRequestMessage.Method == HttpMethod.Head ||
                                       (targetRequestMessage.Method == HttpMethod.Delete && responseMessage.StatusCode == HttpStatusCode.Accepted);

                if (!doNotWriteToBody) {
                    var content = await responseMessage.Content.ReadAsByteArrayAsync();
                    await context.Response.Body.WriteAsync(content);
                }

                var metric = new RoutingMetric {
                    RouteIndex = routeIndex,
                    HttpMethod = targetRequestMessage.Method.Method,
                    Uri = targetUri.AbsoluteUri,
                    StatusCode = (int)responseMessage.StatusCode,
                    Elapsed = sw.Elapsed,
                    //RequestLength = targetRequestMessage.Content.Headers.ContentLength,
                    ResponseLength = responseMessage.Content.Headers.ContentLength
                };
                proxyRouteHandler.End(metric);

                return;
            }

            await next(context);
        });
    }

    private static async Task<HttpResponseMessage> Forward(HttpRequestMessage targetRequestMessage, CancellationToken token) {
        try {
            using var responseMessage = await _httpClient.SendAsync(
                      targetRequestMessage,
                      HttpCompletionOption.ResponseHeadersRead,
                      token);
            return responseMessage;
        }
        catch (Exception exc) {
            var err = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            err.Content = new StringContent(exc.ToString(), System.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/plain"));
            return err;
        }
    }
}