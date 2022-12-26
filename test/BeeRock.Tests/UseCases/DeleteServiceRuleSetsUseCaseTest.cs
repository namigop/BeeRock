using BeeRock.Adapters.UseCases.DeleteServiceRuleSets;
using BeeRock.Tests.UseCases.Fakes;
using Microsoft.AspNetCore.Rewrite;

namespace BeeRock.Tests.UseCases;

[TestClass]
public class DeleteServiceRuleSetsUseCaseTest {
    [TestMethod]
    public async Task Test_that_deleting_a_service_also_deletes_its_rules() {
        var db = new FakeDb();
        var svcRepo = new FakeDocSvcRuleSetsRepo(db);
        var ruleRepo = new FakeDocRuleRepo(db);

        var all = svcRepo.All();
        Assert.AreEqual(10, all.Count);

        var svc = all.First();
        var routeDocId = svc.Routes.First().RuleSetIds.First();

        var d = new DeleteServiceRuleSetsUseCase(svcRepo, ruleRepo);
        await d.Delete(svc.DocId).Match(
            o => {
                var svcExists = svcRepo.Exists(svc.DocId);
                Assert.IsFalse(svcExists);

                var ruleExists = ruleRepo.Exists(routeDocId);
                Assert.IsFalse(ruleExists);
            },
            exception => { Assert.Fail("Delete should not have failed"); });
    }

    [TestMethod]
    public async Task Test_that_delete_all_is_ok() {
        var db = new FakeDb();
        var svcRepo = new FakeDocSvcRuleSetsRepo(db);
        var ruleRepo = new FakeDocRuleRepo(db);

        var services = svcRepo.All();
        Assert.AreEqual(10, services.Count);

        var rules = ruleRepo.All();
        Assert.AreEqual(10, rules.Count);

        var d = new DeleteServiceRuleSetsUseCase(svcRepo, ruleRepo);
        foreach (var svc in services) {
            await d.Delete(svc.DocId).Invoke();
        }

        Assert.AreEqual(0, svcRepo.All().Count);
        Assert.AreEqual(0, ruleRepo.All().Count);
    }
}
