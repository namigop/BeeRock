using Microsoft.AspNetCore.Mvc;

namespace BeeRock.Core.Entities.ReverseProxy;

public class ReverseProxyController : ControllerBase {
    [HttpGet("health")]
    public Task<string> Health() {
        return Task.FromResult("healthy");
    }
}
