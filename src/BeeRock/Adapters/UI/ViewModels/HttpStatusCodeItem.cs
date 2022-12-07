using System.Net;

namespace BeeRock.Adapters.UI.ViewModels;

public class HttpStatusCodeItem {
    private readonly HttpStatusCode _statusCode;

    public HttpStatusCode StatusCode {
        get => _statusCode;
        init {
            _statusCode = value;
            if ((int)value >= 400) DefaultResponse = "//Add a custom error message below";
        }
    }

    public string Display => $"{(int)StatusCode} {StatusCode}";

    public string DefaultResponse { get; set; }
}