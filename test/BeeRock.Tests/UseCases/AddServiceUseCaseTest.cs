using BeeRock.Adapters.UseCases.AddService;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Ports.AddServiceUseCase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace BeeRock.Tests.UseCases;

[TestClass]
public class AddServiceUseCaseTest {
    [TestMethod]
    public async Task Test_that_add_service_is_ok() {
        IRestService CreateService(Type[] types, string name, RestServiceSettings settings) {
            Assert.IsNotNull(types);
            Assert.IsNotNull(name);
            Assert.IsNotNull(settings);
            Assert.IsTrue(types.Any());

            return new TempRestService {
                ControllerTypes = types,
                Name = name,
                Settings = settings,
                Methods = new RestControllerReader().Inspect(types.First())
            };
        }

        ICsCompiler CreateCompiler(string rand, string dll) {
            Assert.IsTrue(!string.IsNullOrEmpty(rand));
            Assert.IsTrue(!string.IsNullOrEmpty(dll));

            return new TempCsCompiler();
        }

        async Task<string> GenerateCode(string rand, string swaggerUrlOrFile) {
            return "some code here";
        }

        var d = new AddServiceUseCase(
            GenerateCode,
            CreateCompiler,
            CreateService
        );

        var addParams = new AddServiceParams { SwaggerUrl = "sdf", Port = 80, ServiceName = "TestService" };
        await d.AddService(addParams)
            .Match(o => {
                    Assert.AreEqual(addParams.ServiceName, o.Name);
                    Assert.AreEqual(1, o.ControllerTypes.Length);
                    Assert.AreEqual(typeof(TestController), o.ControllerTypes[0]);

                    //Validates the generated endpoints
                    Assert.AreEqual(2, o.Methods.Count);
                    Assert.AreEqual("POST", o.Methods[0].HttpMethod);
                    Assert.AreEqual("AddPet", o.Methods[0].MethodName);
                    Assert.AreEqual(typeof(void), o.Methods[0].ReturnType);
                    Assert.AreEqual(1, o.Methods[0].Parameters.Count);
                    Assert.AreEqual("Pet", o.Methods[0].Parameters[0].TypeName);
                    Assert.AreEqual("v2/pet", o.Methods[0].RouteTemplate);

                    Assert.AreEqual("GET", o.Methods[1].HttpMethod);
                    Assert.AreEqual("FindPetsByStatus", o.Methods[1].MethodName);
                    Assert.AreEqual(typeof(List<Pet>), o.Methods[1].ReturnType);
                    Assert.AreEqual(1, o.Methods[1].Parameters.Count);
                    Assert.AreEqual("List<Status>", o.Methods[1].Parameters[0].TypeName);
                },
                exc => { Assert.Fail("Should not reach this part because we already predefined TestController"); });
    }
}

public class TempRestService : IRestService {
    public Type[] ControllerTypes { get; init; } = Array.Empty<Type>();
    public List<RestMethodInfo> Methods { get; init; } = new();
    public string Name { get; init; } = "";
    public string SwaggerUrl { get; init; } = "";
    public RestServiceSettings Settings { get; init; } = new();
    public string DocId { get; set; }
}

public class TempCsCompiler : ICsCompiler {
    public Type[] GetTypes() {
        return new[] { typeof(TestController) };
    }

    public List<string> CompilationErrors { get; } = new();
    public string[] SourceCodeStrings { get; } = Array.Empty<string>();
    public bool Success { get; } = true;
    public OutputKind TargetOutput { get; } = OutputKind.DynamicallyLinkedLibrary;

    public void Compile(params MetadataReference[] additionalReferences) {
        //do nothing. We already have the TestController Type
    }
}

public class Pet {
    public string Name { get; set; }
}

public enum Status {
    Alive = 0,
    Kicking = 1
}

[Route("v2")]
public class TestController : ControllerBase {
    [HttpPost]
    [Route("pet")]
    public Task AddPet([FromBody] Pet body) {
        return Task.CompletedTask;
    }

    [HttpGet]
    [Route("pet/findByStatus")]
    public Task<List<Pet>> FindPetsByStatus([FromQuery] List<Status> status) {
        var pets = new List<Pet> { new() { Name = "foo" }, new() { Name = "bar" } };
        return Task.FromResult(pets);
    }
}
