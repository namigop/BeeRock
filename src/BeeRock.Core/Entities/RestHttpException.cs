using System.Net;

namespace BeeRock.Core.Entities;

public class RestHttpException : Exception {
    public HttpStatusCode StatusCode { get; init; }
    public string Error { get; init; }
}