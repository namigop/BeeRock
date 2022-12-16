using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities;

public class ScriptingHttpHeader {
    private readonly IHeaderDictionary _h;

    public ScriptingHttpHeader(IHeaderDictionary h) {
        _h = h;
    }
    public string Get(string header) {
        if (_h.ContainsKey(header))
            return _h[header];

        return "invalid header value";
    }
}
