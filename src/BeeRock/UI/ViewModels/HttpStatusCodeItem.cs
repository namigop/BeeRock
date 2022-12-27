using System.Net;

namespace BeeRock.UI.ViewModels;

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

    public string Color {
        get {
            if ((int)_statusCode >= 200 && (int)_statusCode < 400)
                return HttpMethodColor.Get;
            if ((int)_statusCode >= 400 && (int)_statusCode < 500)
                return HttpMethodColor.Put;

            return HttpMethodColor.Delete;
        }
    }

    public string DefaultResponse { get; set; }
}