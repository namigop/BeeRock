using Microsoft.AspNetCore.Mvc;

namespace BeeRock.Tests.UseCases.TestArtifacts;

[Route("v2")]
public class UnitTestController : ControllerBase {
    [HttpPost]
    [Route("pet")]
    public Task AddPet([FromBody] UnitTestPet body) {
        return Task.CompletedTask;
    }

    [HttpGet]
    [Route("pet/findByStatus")]
    public Task<List<UnitTestPet>> FindPetsByStatus([FromQuery] List<UnitTestStatus> status) {
        var pets = new List<UnitTestPet> { new() { Name = "foo" }, new() { Name = "bar" } };
        return Task.FromResult(pets);
    }
}
