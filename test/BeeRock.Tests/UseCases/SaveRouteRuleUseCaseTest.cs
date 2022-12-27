using BeeRock.Core.Entities;
using BeeRock.Core.UseCases.SaveRouteRule;
using BeeRock.Tests.UseCases.Fakes;

namespace BeeRock.Tests.UseCases;

[TestClass]
public class SaveRouteRuleUseCaseTest {
    [TestMethod]
    public async Task Test_that_new_rules_get_assigned_an_id() {
        var db = new FakeDb();
        var svcRepo = new FakeDocSvcRuleSetsRepo(db);
        var ruleRepo = new FakeDocRuleRepo(db);

        var svc = db.svcDb.Values.Skip(5).First();
        var prevRule = db.ruleDb[svc.Routes[0].RuleSetIds[0]];

        var rule = new Rule {
            Body = "//some json",
            Name = "Rule 01",
            Conditions = new[] { new WhenCondition { BoolExpression = "True", IsActive = true } },
            DelayMsec = 500,
            DocId = null,
            IsSelected = true,
            StatusCode = 404
        };

        var uc = new SaveRouteRuleUseCase(ruleRepo);
        await uc.Save(rule).Match(
            docId => {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(docId));
                Assert.AreEqual(rule.LastUpdated.Date, DateTime.Now.Date);
            },
            exception => Assert.Fail("Save rule should not have failed"));
    }

    [TestMethod]
    public async Task Test_that_existing_rules_can_be_updated() {
        var db = new FakeDb();
        var svcRepo = new FakeDocSvcRuleSetsRepo(db);
        var ruleRepo = new FakeDocRuleRepo(db);

        var svc = db.svcDb.Values.Skip(5).First();
        var prevRule = db.ruleDb[svc.Routes[0].RuleSetIds[0]];

        var rule = new Rule {
            Body = prevRule.Body,
            Name = prevRule.Name,
            Conditions = prevRule.Conditions.Select(c => new WhenCondition { BoolExpression = c.BooleanExpression, IsActive = c.IsActive })
                .ToArray(),
            DelayMsec = prevRule.DelayMsec,
            DocId = prevRule.DocId,
            IsSelected = prevRule.IsSelected,
            StatusCode = prevRule.StatusCode
        };

        var uc = new SaveRouteRuleUseCase(ruleRepo);
        await uc.Save(rule).Match(
            docId => {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(docId));
                Assert.AreEqual(rule.LastUpdated.Date, DateTime.Now.Date);
                Assert.AreEqual(prevRule.DocId, docId);
            },
            exception => Assert.Fail("Save rule should not have failed"));
    }
}
