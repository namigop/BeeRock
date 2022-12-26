using Microsoft.AspNetCore.Mvc;

namespace BeeRock.Tests.UseCases.Fakes;

[Route("v2")]
public class FakeController : ControllerBase {
    [HttpPost]
    [Route("pet")]
    public Task AddPet([FromBody] FakePet body) {
        return Task.CompletedTask;
    }

    [HttpGet]
    [Route("pet/findByStatus")]
    public Task<List<FakePet>> FindPetsByStatus([FromQuery] List<FakeStatus> status) {
        var pets = new List<FakePet> { new() { Name = "foo" }, new() { Name = "bar" } };
        return Task.FromResult(pets);
    }
}
