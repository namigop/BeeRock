using BeeRock.Ports.Repository;

namespace BeeRock.Tests.UseCases.TestArtifacts;

public class UnitTestDb {
    public readonly Dictionary<string, DocServiceRuleSetsDao> svcDb = new();
    public readonly Dictionary<string, DocRuleDao> ruleDb = new();

    public UnitTestDb() {

        //Test data. 1 rule set per service
        for (int i = 0; i < 10; i++) {
            var id = Guid.NewGuid().ToString();
            var ruleId = Guid.NewGuid().ToString();

            svcDb.Add(id, new DocServiceRuleSetsDao() {
                DocId = id,
                ServiceName = $"Doc-{i}",
                PortNumber = 8000 + i,
                SourceSwagger = $"http:/path/to/swagger/{i}/index.html",
                Routes = new[] { new RouteRuleSetsDao() { RouteTemplate = $"path/{i}",  RuleSetIds = new[] { ruleId } } }
            });

            ruleDb.Add(ruleId, new DocRuleDao() {
                DocId = ruleId,
                Name = $"Doc-{i}",
                StatusCode = 200,
                Body = $"some json {i}",
                DelayMsec = 50,
                Conditions = new []{ new WhenDao(){ BooleanExpression = "1 == 1", IsActive = true}}
            });

        }
    }
}