using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.AddService;
using BeeRock.Tests.UseCases.Fakes;

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

            return new FakeRestService {
                ControllerTypes = types,
                Name = name,
                Settings = settings,
                Methods = new RestControllerReader().Inspect(types.First())
            };
        }

        ICsCompiler CreateCompiler(string rand, string dll) {
            Assert.IsTrue(!string.IsNullOrEmpty(rand));
            Assert.IsTrue(!string.IsNullOrEmpty(dll));

            return new FakeCsCompiler();
        }

        Task<string> GenerateCode(string rand, string swaggerUrlOrFile) {
            return Task.FromResult("some code here");
        }

        var d = new AddServiceUseCase(
            GenerateCode,
            CreateCompiler,
            CreateService
        );

        var addParams = new AddServiceParams { SwaggerUrl = "sdf", Port = 80, ServiceName = "TestService", TempPath = "." };
        await d.AddService(addParams)
            .Match(o => {
                    Assert.AreEqual(addParams.ServiceName, o.Name);
                    Assert.AreEqual(1, o.ControllerTypes.Length);
                    Assert.AreEqual(typeof(FakeController), o.ControllerTypes[0]);

                    //Validates the generated endpoints
                    Assert.AreEqual(2, o.Methods.Count);
                    Assert.AreEqual("POST", o.Methods[0].HttpMethod);
                    Assert.AreEqual("AddPet", o.Methods[0].MethodName);
                    Assert.AreEqual(typeof(void), o.Methods[0].ReturnType);
                    Assert.AreEqual(6, o.Methods[0].Parameters.Count);
                    Assert.AreEqual("bee.Proxy", o.Methods[0].Parameters.Last().Name);
                    Assert.AreEqual("FakePet", o.Methods[0].Parameters[0].TypeName);
                    Assert.AreEqual("v2/pet", o.Methods[0].RouteTemplate);

                    Assert.AreEqual("GET", o.Methods[1].HttpMethod);
                    Assert.AreEqual("FindPetsByStatus", o.Methods[1].MethodName);
                    Assert.AreEqual(typeof(List<FakePet>), o.Methods[1].ReturnType);
                    Assert.AreEqual(6, o.Methods[1].Parameters.Count);
                    Assert.AreEqual("List<FakeStatus>", o.Methods[1].Parameters[0].TypeName);
                },
                exc => { Assert.Fail("Should not reach this part because we already predefined TestController"); });
    }
}
