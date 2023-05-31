using System.Net;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarContext {
    private readonly HttpContext _ctx;
  
    public ScriptingVarContext(HttpContext ctx, string requestBody) {
        _ctx = ctx;
         
        if (ctx != null) {
            Request = new Req { HttpRequest = ctx.Request, RequestBody = requestBody };
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
        private JObject jObject;
        private XmlDocument xmlDoc;
        public HttpRequest HttpRequest { get; init; }
        public string RequestBody { get; init; }

        public string Get(string jxpath) {
            if (string.IsNullOrWhiteSpace(jxpath))
                return "supplied {path} is empty";

            if (string.IsNullOrEmpty(this.RequestBody))
                return "<empty>";
            
            if (this.HttpRequest.ContentType?.Contains("/json") ?? false) {
                this.jObject ??= JObject.Parse(this.RequestBody);
            }

            if (jObject != null) {
                return jObject.SelectToken(jxpath).ToString();
            }
            
            if (this.HttpRequest.ContentType?.Contains("/xml") ?? false) {
                this.xmlDoc ??= new XmlDocument();
                this.xmlDoc.LoadXml(this.RequestBody);
            }

            if (xmlDoc != null) {
                return this.xmlDoc.SelectSingleNode(jxpath).InnerXml;
            }


            throw new Exception("Unable to query the http request body. Supported content types are /json or /xml");
        }
    }
}
