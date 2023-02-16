using Microsoft.AspNetCore.Http;

// ReSharper disable UnusedMember.Global.  The methods here will be called in the python script

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingHttpHeader {
    public ScriptingHttpHeader(IHeaderDictionary reqHeaders, IHeaderDictionary respHeaders) {
        Request = new Req { Headers = reqHeaders };
        Response = new Resp { Headers = respHeaders };
    }

    public Req Request { get; }
    public Resp Response { get; }

    private static string Get(IHeaderDictionary headers, string key) {
        if (headers.ContainsKey(key))
            return headers[key];

        var all = string.Join(", ", headers.Keys);
        return $"Header {key} not found. Current headers are : {all}";
    }

    private static void Add(IHeaderDictionary headers, string key, string value) {
        if (headers.ContainsKey(key))
            headers[key] = value;
        else
            headers.TryAdd(key, value);
    }

    private static void Remove(IHeaderDictionary headers, string key) {
        if (headers.ContainsKey(key))
            headers.Remove(key);
    }

    public class Req {
        public IHeaderDictionary Headers { get; init; }

        public string Get(string header) {
            return ScriptingHttpHeader.Get(Headers, header);
        }

        public void Add(string key, string value) {
            ScriptingHttpHeader.Add(Headers, key, value);
        }

        public void Remove(string key) {
            ScriptingHttpHeader.Remove(Headers, key);
        }
    }

    public class Resp {
        public IHeaderDictionary Headers { get; init; }

        public string Get(string header) {
            return ScriptingHttpHeader.Get(Headers, header);
        }

        public void Add(string key, string value) {
            ScriptingHttpHeader.Add(Headers, key, value);
        }

        public void Remove(string key) {
            ScriptingHttpHeader.Remove(Headers, key);
        }
    }
}
