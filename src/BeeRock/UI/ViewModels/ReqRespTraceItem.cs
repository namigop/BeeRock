using Avalonia.Collections;
using BeeRock.Core.Dtos;
using BeeRock.Core.Utils;
using DynamicData;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ReqRespTraceItem : ViewModelBase {
    private uint _elapsedMsec;
    private DateTime _timestamp;
    private string _requestBody;
    private string _responseBody;
    private List<Pair<string, string>> _requestHeaders;
    private List<Pair<string, string>> _responseHeaders;
    private string _requestUri;
    private string _statusCode;
    private string _docId;
    private DateTime _lastUpdated;
    private string _requestMethod;

    public ReqRespTraceItem(DocReqRespTraceDto dto) {
        _elapsedMsec = dto.ElapsedMsec;
        _timestamp = dto.Timestamp;
        _requestBody =  dto.RequestBody;
        _responseBody = dto.ResponseBody;
        _requestUri = dto.RequestUri;
        _statusCode = dto.StatusCode;
        _docId = dto.DocId;
        _lastUpdated = dto.LastUpdated;
        _requestMethod = dto.RequestMethod;

        _requestHeaders = new List<Pair<string, string>>();

        foreach (var kvp in dto.RequestHeaders)
            _requestHeaders.Add( new Pair<string, string>(){Key = kvp.Key, Value = kvp.Value});

        _responseHeaders = new List<Pair<string, string>>();
        foreach (var kvp in dto.ResponseHeaders)
            _responseHeaders.Add( new Pair<string, string>(){Key = kvp.Key, Value = kvp.Value});

        this.PathAndQuery = new Uri(this.RequestUri).PathAndQuery;

        _requestHeaders.Insert(0, new Pair<string, string>(){Key = "Request Uri", Value = _requestUri});
        //_responseHeaders.Insert(0, new Pair<string, string>(){Key = "Elapsed (msec)", Value = $"{_elapsedMsec} msec"});


    }

    public string PathAndQuery { get; private set; }

    public uint ElapsedMsec {
        get => _elapsedMsec;
        set => this.RaiseAndSetIfChanged(ref _elapsedMsec , value);
    }

    public DateTime Timestamp {
        get => _timestamp;
        set => this.RaiseAndSetIfChanged(ref _timestamp, value);
    }

    public string RequestBody {
        get => _requestBody;
        set => this.RaiseAndSetIfChanged(ref _requestBody, value);
    }

    public string ResponseBody {
        get => _responseBody;
        set => this.RaiseAndSetIfChanged(ref _responseBody, value);
    }

    public List<Pair<string, string>> RequestHeaders {
        get => _requestHeaders;
        set => this.RaiseAndSetIfChanged(ref _requestHeaders, value);
    }

    public List<Pair<string, string>> ResponseHeaders {
        get => _responseHeaders;
        set => this.RaiseAndSetIfChanged(ref _responseHeaders, value);
    }

    public string RequestUri {
        get => _requestUri;
        set => this.RaiseAndSetIfChanged(ref _requestUri, value);
    }

    public string StatusCode {
        get => _statusCode;
        set => this.RaiseAndSetIfChanged(ref _statusCode, value);
    }

    public string DocId {
        get => _docId;
        set => this.RaiseAndSetIfChanged(ref _docId, value);
    }

    public DateTime LastUpdated {
        get => _lastUpdated;
        set => this.RaiseAndSetIfChanged(ref _lastUpdated, value);
    }

    public string Color {
        get {
            if (RequestMethod == "POST")
                return HttpMethodColor.Post;
            if (RequestMethod == "PUT")
                return HttpMethodColor.Put;
            if (RequestMethod == "DELETE")
                return HttpMethodColor.Delete;
            if (RequestMethod == "PATCH")
                return HttpMethodColor.Patch;

            return HttpMethodColor.Get;
        }
    }
    public string StatusColor {
        get {
            var stat = Convert.ToUInt32(StatusCode);
            if ( stat is >= 200 and < 400)
                return HttpMethodColor.Post;
            else if ( stat is >= 400 and < 500)
                return HttpMethodColor.Put;
            else if ( stat is >= 500)
                return HttpMethodColor.Delete;
            else {
                return HttpMethodColor.Get;
            }
        }
    }

    public string RequestMethod {
        get => _requestMethod;
        set => this.RaiseAndSetIfChanged(ref _requestMethod, value);
    }

    public void PrettyPrint() {
        if (string.IsNullOrWhiteSpace(this.RequestBody))
            this.RequestBody = "//No content";
        if (string.IsNullOrWhiteSpace(this.ResponseBody))
            this.ResponseBody = "//No content";

        this.RequestBody = Helper.PrettyPrint(this.RequestBody);
        this.ResponseBody = Helper.PrettyPrint(this.ResponseBody);
    }
}
