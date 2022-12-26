using BeeRock.Tests.UseCases.TestArtifacts;

namespace BeeRock.Tests.UseCases;

[TestClass]
public class SaveServiceDetailsUseCaseTest {
    [TestMethod]
    public async Task Test_that_new_rules_get_assigned_an_id() {
        var db = new UnitTestDb();
        var svcRepo = new UnitTestDocSvcRuleSetsRepo(db);
        var ruleRepo = new UnitTestDocRuleRepo(db);
        //TODO
    }
}
