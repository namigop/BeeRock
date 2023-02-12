using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace BeeRock.Core.Entities.ReverseProxy;

public static class ReverseProxyMiddleware {
    private static readonly HttpClient _httpClient = new();

    private static readonly List<string> contentheaders = new() {
        "Allow",
        "Content-Disposition",
        "Content-Encoding",
        "Content-Language",
        "Content-Length",
        "Content-Location",
        "Content-MD5",
        "Content-Range",
        "Content-Type",
        "Expires",
        "Last-Modified"
    };

    public static void ConfigureReverseProxy(this IApplicationBuilder app, IProxyRouteSelector proxyRouteSelector) {
        app.Use(async (context, next) => {
            var sourceUri = new Uri(UriHelper.GetEncodedUrl(context.Request));
            var targetUri = proxyRouteSelector.BuildUri(sourceUri); //   BuildTargetUri(context.Request, "https://scl-apigateway.cxos.tech");
            if (targetUri != null) {
                C.Info($"Routing HTTP {context.Request.Method}");
                C.Info($"From: {sourceUri}");
                C.Info($"  To: {targetUri}");

                var targetRequestMessage = CreateTargetMessage(context, targetUri);

                using var responseMessage = await _httpClient.SendAsync(
                    targetRequestMessage,
                    HttpCompletionOption.ResponseHeadersRead,
                    context.RequestAborted);

                context.Response.StatusCode = (int)responseMessage.StatusCode;
                CopyResponseHeaders(context, responseMessage);
                var content = await responseMessage.Content.ReadAsByteArrayAsync();
                await context.Response.Body.WriteAsync(content);
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

    private static HttpMethod GetMethod(string method) {
        if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
        if (HttpMethods.IsGet(method)) return HttpMethod.Get;
        if (HttpMethods.IsHead(method)) return HttpMethod.Head;
        if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
        if (HttpMethods.IsPost(method)) return HttpMethod.Post;
        if (HttpMethods.IsPut(method)) return HttpMethod.Put;
        if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
        return new HttpMethod(method);
    }

    private static void CopyRequestHeaders(HttpContext context, HttpRequestMessage requestMessage) {
        var requestMethod = context.Request.Method;

        if (!HttpMethods.IsGet(requestMethod) &&
            !HttpMethods.IsHead(requestMethod) &&
            !HttpMethods.IsDelete(requestMethod) &&
            !HttpMethods.IsTrace(requestMethod)) {
            var streamContent = new StreamContent(context.Request.Body);
            requestMessage.Content = streamContent;
        }


        foreach (var header in context.Request.Headers)
            if (contentheaders.Contains(header.Key))
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            else
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
    }

    private static void CopyRequestContent(HttpContext context, HttpRequestMessage requestMessage) {
        var requestMethod = context.Request.Method;

        if (!HttpMethods.IsGet(requestMethod) &&
            !HttpMethods.IsHead(requestMethod) &&
            !HttpMethods.IsDelete(requestMethod) &&
            !HttpMethods.IsTrace(requestMethod)) {
            var streamContent = new StreamContent(context.Request.Body);
            requestMessage.Content = streamContent;
        }
    }

    private static HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri) {
        var requestMessage = new HttpRequestMessage();

        CopyRequestContent(context, requestMessage);
        CopyRequestHeaders(context, requestMessage);

        requestMessage.RequestUri = targetUri;
        requestMessage.Headers.Host = targetUri.Host;
        requestMessage.Method = GetMethod(context.Request.Method);

        return requestMessage;
    }

    private static Uri BuildTargetUri(HttpRequest request, string targetServer) {
       var s = UriHelper.GetEncodedUrl(request);
        if (request.QueryString.HasValue) {
            return new Uri(targetServer + request.Path + request.QueryString);
        }

        return new Uri(targetServer + request.Path);
    }
}
