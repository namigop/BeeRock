using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Middlewares {
    public static class HttpUtil {

        public static IEnumerable<string> Contentheaders = new List<string>() {
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

        public static void CopyRequestHeaders(HttpRequest source, HttpRequestMessage requestMessage) {
            var requestMethod = source.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod)) {
                var streamContent = new StreamContent(source.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in source.Headers)
                if (Contentheaders.Contains(header.Key))
                    _ = requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                else
                    _ = requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        public static void CopyResponseHeaders(HttpResponse source, HttpResponseMessage responseMessage) {
            foreach (var header in responseMessage.Headers) {
                source.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers) {
                source.Headers[header.Key] = header.Value.ToArray();
            }

            _ = source.Headers.Remove("transfer-encoding");
        }

        public static HttpRequestMessage CreateRequestMessage(HttpRequest source, Uri targetUri) {
            var requestMessage = new HttpRequestMessage();

            CopyRequestContent(source, requestMessage);
            CopyRequestHeaders(source, requestMessage);

            requestMessage.RequestUri = targetUri;
            requestMessage.Headers.Host = targetUri.Host;
            requestMessage.Method = GetMethod(source.Method);

            return requestMessage;
        }

        public static HttpMethod GetMethod(string method) {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }

        private static void CopyRequestContent(HttpRequest source, HttpRequestMessage requestMessage) {
            var requestMethod = source.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod)) {
                var streamContent = new StreamContent(source.Body);
                requestMessage.Content = streamContent;
            }
        }
    }
}
