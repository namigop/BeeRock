using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingQueryString {
    private readonly IQueryCollection _query;

    public ScriptingQueryString(IQueryCollection query) {
        _query = query;
    }

    public string Get(string key) {
        if (_query.TryGetValue(key, out var vals)) return vals.ToString();

        return "";
    }
}