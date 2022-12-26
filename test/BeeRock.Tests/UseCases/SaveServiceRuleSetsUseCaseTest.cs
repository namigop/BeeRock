using BeeRock.Adapters.UseCases.SaveServiceRuleSets;
using BeeRock.Core.Entities;
using BeeRock.Tests.UseCases.Fakes;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace BeeRock.Tests.UseCases;

[TestClass]
public class SaveServiceRuleSetsUseCaseTest {
    [TestMethod]
    public async Task Test_that_new_services_can_be_created() {
        var db = new FakeDb();
        var svcRepo = new FakeDocSvcRuleSetsRepo(db);
        var ruleRepo = new FakeDocRuleRepo(db);

        var uc = new SaveServiceRuleSetsUseCase(svcRepo, ruleRepo);

        var types = new[] { typeof(FakeController) };
        var svcName = "FakeRestService";
        var svc = new RestService(types, "FakeRestService",
            new RestServiceSettings() {
                Enabled = true,
                PortNumber = 12345,
                SourceSwaggerDoc = "http://myswagger/doc.json"
            });


        await uc.Save(svc).Match(
            docId => {
                var dao = svcRepo.Read(docId);
                Assert.AreEqual(svcName, dao.ServiceName);
                Assert.AreEqual(svc.Settings.PortNumber, dao.PortNumber);
                Assert.AreEqual(svc.Settings.SourceSwaggerDoc, dao.SourceSwagger);
            },
            exc => { });

    }
}
