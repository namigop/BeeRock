using System.Net;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarContext {
    private readonly HttpContext _ctx;

    public ScriptingVarContext(HttpContext ctx) {
        _ctx = ctx;
        if (ctx != null) {
            Request = new Req { HttpRequest = ctx.Request };
            Response = new Resp { HttpResponse = ctx.Response };
            Items = ctx.Items;
            Headers = new ScriptingHttpHeader(ctx.Request.Headers, ctx.Response.Headers);
            QueryString = new ScriptingQueryString(ctx.Request.Query);
        }
    }

    public ScriptingQueryString QueryString { get; init; }
    public ScriptingHttpHeader Headers { get; init; }

    public IDictionary<object, object> Items { get; }

    public Resp Response { get; }

    public Req Request { get; }

    public bool ContainsPassThroughResponse => Items.ContainsKey(nameof(PassThroughResponse));

    public void Capture(object result) {
        if (Response.IsPassThrough) {
            if (result is not string)
                throw new Exception("Only string (text) is supported for PassThrough responses");

            Items[nameof(PassThroughResponse)] = new PassThroughResponse {
                Content = result == null ? "" : result.ToString(),
                ContentType = Response.ContentType,
                StatusCode = Response.StatusCode
            };
        }
    }


    public class Resp {
        private static readonly int[] intCodes = Enum.GetValues(typeof(HttpStatusCode)).Cast<int>().ToArray();
        public HttpResponse HttpResponse { get; init; }

        public string ContentType { get; private set; } = "application/json";
        public bool IsPassThrough { get; private set; }
        public int StatusCode { get; private set; } = 200;

        public void SetContentType(string contentType) {
            ContentType = contentType;
        }

        public void SetStatusCode(int statusCode) {
            if (!intCodes.Contains(statusCode))
                throw new Exception($"Unable to assign an invalid status code value of {statusCode}");

            StatusCode = statusCode;
        }

        public void SetAsPassThrough(bool isPassThrough = true) {
            IsPassThrough = isPassThrough;
        }
    }

    public class Req {
        public HttpRequest HttpRequest { get; init; }
    }
}
