using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities;

public class ScriptingHttpHeader {
    public ScriptingHttpHeader(IHeaderDictionary h) {
        Headers = h;
    }

    public IHeaderDictionary Headers { get; }

    public string Get(string header) {
        if (Headers.ContainsKey(header))
            return Headers[header];

        return "invalid header value";
    }
}
