using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingHttpHeader {
    public ScriptingHttpHeader(IHeaderDictionary reqHeaders, IHeaderDictionary respHeaders) {
        Request = new Req { Headers = reqHeaders };
        Response = new Resp { Headers = respHeaders };
    }

    public Req Request { get; }
    public Resp Response { get; }


    public class Req {
        public IHeaderDictionary Headers { get; init; }

        public string Get(string header) {
            if (Headers.ContainsKey(header))
                return Headers[header];

            return "invalid header value";
        }
    }

    public class Resp {
        public IHeaderDictionary Headers { get; init; }

        public string Get(string header) {
            if (Headers.ContainsKey(header))
                return Headers[header];

            return "invalid header value";
        }
    }
}
