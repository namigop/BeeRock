using System.Net;

namespace BeeRock.Core.Entities;

/// <summary>
///     Exception thrown when returning a non-200 OK response
/// </summary>
public class RestHttpException : Exception {
    public HttpStatusCode StatusCode { get; init; }
    public string Error { get; init; }
}
