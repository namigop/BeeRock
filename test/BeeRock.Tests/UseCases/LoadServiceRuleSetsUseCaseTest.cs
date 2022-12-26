using BeeRock.Adapters.UseCases.LoadServiceRuleSets;
using BeeRock.Core.Interfaces;
using BeeRock.Ports.Repository;
using BeeRock.Tests.UseCases.Fakes;

namespace BeeRock.Tests.UseCases;

[TestClass]
public class LoadServiceRuleSetsUseCaseTest {

    [TestMethod]
    public async Task Test_that_service_can_be_loaded_by_id() {
        var db = new FakeDb();
        var svcRepo = new FakeDocSvcRuleSetsRepo(db);
        var ruleRepo = new FakeDocRuleRepo(db);

        var svc = db.svcDb.Values.Skip(5).First();
        var rule = db.ruleDb[svc.Routes[0].RuleSetIds[0]];

        var uc = new LoadServiceRuleSetsUseCase(svcRepo, ruleRepo);
        await uc.LoadById(svc.DocId).Match(
            o => Validate(svc, o, rule),
            exception => Assert.Fail("LoadById should not have failed"));
    }

    [TestMethod]
    public async Task Test_that_service_can_be_loaded_by_swagger_and_name() {
        var db = new FakeDb();
        var svcRepo = new FakeDocSvcRuleSetsRepo(db);
        var ruleRepo = new FakeDocRuleRepo(db);

        var svc = db.svcDb.Values.Skip(8).First();
        var rule = db.ruleDb[svc.Routes[0].RuleSetIds[0]];

        var uc = new LoadServiceRuleSetsUseCase(svcRepo, ruleRepo);
        await uc.LoadBySwaggerAndName(svc.ServiceName, svc.SourceSwagger).Match(
            o =>  Validate(svc, o, rule),
            exception => Assert.Fail("LoadById should not have failed"));
    }

    private static void Validate(DocServiceRuleSetsDao svc, IRestService o, DocRuleDao rule) {
        //Validate the svc is loaded correctly
        Assert.AreEqual(svc.DocId, o.DocId);
        Assert.AreEqual(svc.Routes.Length, o.Methods.Count);
        Assert.AreEqual(svc.ServiceName, o.Name);
        Assert.AreEqual(svc.SourceSwagger, o.Settings.SourceSwaggerDoc);
        Assert.AreEqual(svc.PortNumber, o.Settings.PortNumber);

        //validate the rules are loaded correctly. Note: there's only 1 rule, hence the [0] indexing
        Assert.AreEqual(rule.DocId, o.Methods[0].Rules[0].DocId);
        Assert.AreEqual(rule.Name, o.Methods[0].Rules[0].Name);
        Assert.AreEqual(rule.Body, o.Methods[0].Rules[0].Body);
        Assert.AreEqual(rule.DelayMsec, o.Methods[0].Rules[0].DelayMsec);
        Assert.AreEqual(rule.Conditions[0].BooleanExpression, o.Methods[0].Rules[0].Conditions[0].BoolExpression);
        Assert.AreEqual(rule.StatusCode, o.Methods[0].Rules[0].StatusCode);
    }
}
