using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Middlewares;

public static class RestPassThroughMiddleware {
    /// <summary>
    ///     Rewrite the response when a passthrough key is found in the context.Items.
    ///     Passthrough is used when the swagger docs have types like "result:{}" which
    ///     just gets converted in to a System.Object in the generated server code.  For this
    ///     types of responses, we will allow whatever it is the user specified.
    /// </summary>
    public static void CheckForPassThroughResponses(this IApplicationBuilder app) {
        app.Use(async (context, next) => {
            //replace the response stream with a memory stream so that we can rewrite it if needed.
            using var memoryStream = new MemoryStream();
            var originalStream = context.Response.Body;
            context.Response.Body = memoryStream;

            await next(context);

            if (context.Items.ContainsKey(nameof(PassThroughResponse))) {
                var passThroughResp = (PassThroughResponse)context.Items[nameof(PassThroughResponse)];
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.SetLength(0);

                //rewrite the response
                await context.Response.WriteAsync(passThroughResp.Content);
                context.Response.ContentType = passThroughResp.ContentType;
                context.Response.StatusCode = passThroughResp.StatusCode;
            }

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalStream);
            context.Response.Body = originalStream;
        });
    }
}
