using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarContext {
    private readonly HttpContext _ctx;

    public ScriptingVarContext(HttpContext ctx) {
        _ctx = ctx;
        Request = new Req() { HttpRequest = ctx?.Request };
        Response = new Resp() { HttpResponse = ctx?.Response };
        Items = ctx?.Items;
    }

    public IDictionary<object, object> Items { get; }

    public Resp Response { get; }

    public Req Request { get; }

    // public bool IsPassThrough => Items.ContainsKey(nameof(PassThroughResponse));

    public void Capture(object result) {
        if (this.Response.IsPassThrough) {
            if (result is not string)
                throw new Exception("Only string (text) is supported for PassThrough responses");

            Items[nameof(PassThroughResponse)] = new PassThroughResponse {
                Content = result == null ? "" : (this.Response.ContentType.TrimEnd().EndsWith("/json") ? result.ToString().Trim() : result.ToString()),
                ContentType = this.Response.ContentType,
                StatusCode = this.Response.StatusCode
            };
        }
    }


    public class Resp {
        public HttpResponse HttpResponse { get; init; }

        public string ContentType { get; set; } = "application/json";
        public bool IsPassThrough { get; set; } = false;
        public int StatusCode { get; set; } = 200;
    }

    public class Req {
        public HttpRequest HttpRequest { get; init; }
    }
}
