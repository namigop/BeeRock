using Microsoft.AspNetCore.Mvc;

namespace BeeRock.Core.Entities.ReverseProxy;

/// <summary>
/// This is just a dummy controller. The main logic is handled by the ReverseProxyMiddleware
/// </summary>
public class ReverseProxyDummyController : ControllerBase {
    [HttpGet("health")]
    public Task<string> Health() {
        return Task.FromResult("healthy");
    }
}
