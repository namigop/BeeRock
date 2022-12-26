using BeeRock.Adapters.UseCases.SaveServiceDetails;
using BeeRock.Tests.UseCases.Fakes;

namespace BeeRock.Tests.UseCases;

[TestClass]
public class SaveServiceDetailsUseCaseTest {
    [TestMethod]
    public async Task Test_that_service_details_can_be_updated() {
        var db = new FakeDb();
        var svcRepo = new FakeDocSvcRuleSetsRepo(db);


        var svc = svcRepo.All().First();
        var newswagger = "foo";
        var newport = 123;
        var newname = "bar";

        var uc = new SaveServiceDetailsUseCase(svcRepo);
        await uc.Save(svc.DocId, newname, newport, newswagger)
            .Match(
                _ => {
                    var svc2 = svcRepo.Read(svc.DocId);
                    Assert.AreEqual(newport, svc.PortNumber);
                    Assert.AreEqual(newswagger, svc.SourceSwagger);
                    Assert.AreEqual(newname, svc.ServiceName);
                },
                exc => { Assert.Fail("Save service should not have failed"); }
            );
    }
}
