using System.Diagnostics;
using BeeRock.Core.Dtos;
using BeeRock.Core.Entities.Tracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace BeeRock.Core.Entities.Middlewares;

public static class ReqRespTracingMiddleware {
    public static void ConfigureReqRespTracing(this IApplicationBuilder app) {
        var handler = ReqRespTracer.Instance.Value;
        app.Use(async (context, next) => {
            var requestBody = await HttpUtil.BufferRequestBody(context.Request);
            var sourceUri = new Uri(context.Request.GetEncodedUrl());

            //Swap the response body with a memory stream that we can read
            using var memoryStream = new MemoryStream();
            var originalStream = context.Response.Body;
            context.Response.Body = memoryStream;

            var sw = Stopwatch.StartNew();
            await next(context);
            sw.Stop();

            //Copy back to the original response stream
            _ = context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalStream);
            context.Response.Body = originalStream;

            memoryStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            var respBody = await reader.ReadToEndAsync();

            Trace(handler, sourceUri, context.Request.Method, requestBody, respBody, context.Request.Headers, context.Response.Headers, context.Response.StatusCode, sw.ElapsedMilliseconds);
        });
    }

    private static void Trace(ReqRespTracer tracer,
        Uri sourceUri,
        string requestMethod,
        string requestBody,
        string respBody,
        IHeaderDictionary requestHeaders,
        IHeaderDictionary responseHeaders,
        int responseStatusCode,
        long swElapsedMilliseconds) {
        var dto = new DocReqRespTraceDto() {
            Timestamp = DateTime.Now,
            ElapsedMsec = Convert.ToUInt32(swElapsedMilliseconds),
            RequestBody = requestBody,
            ResponseBody = respBody,
            LastUpdated = DateTime.Now,
            RequestUri = sourceUri.AbsoluteUri,
            StatusCode = responseStatusCode.ToString(),
            RequestMethod = requestMethod,
        };

        var reqHeaders = new Dictionary<string, string>();
        foreach (var h in requestHeaders)
            reqHeaders.Add(h.Key, h.Value.ToString());

        var respHeaders = new Dictionary<string, string>();
        foreach (var h in responseHeaders)
            respHeaders.Add(h.Key, h.Value.ToString());

        dto.RequestHeaders = reqHeaders;
        dto.ResponseHeaders = respHeaders;

        tracer.Trace(dto);
    }
}
