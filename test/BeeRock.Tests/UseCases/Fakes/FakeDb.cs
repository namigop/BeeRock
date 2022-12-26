using BeeRock.Core.Dtos;
using BeeRock.Ports.Repository;

namespace BeeRock.Tests.UseCases.Fakes;

public class FakeDb {
    public readonly Dictionary<string, DocServiceRuleSetsDto> svcDb = new();
    public readonly Dictionary<string, DocRuleDto> ruleDb = new();

    public FakeDb() {

        //Test data. 1 rule set per service
        for (int i = 0; i < 10; i++) {
            var id = Guid.NewGuid().ToString();
            var ruleId = Guid.NewGuid().ToString();

            svcDb.Add(id, new DocServiceRuleSetsDto() {
                DocId = id,
                ServiceName = $"Doc-{i}",
                PortNumber = 8000 + i,
                SourceSwagger = $"http:/path/to/swagger/{i}/index.html",
                Routes = new[] { new RouteRuleSetsDto() { RouteTemplate = $"path/{i}",  RuleSetIds = new[] { ruleId } } }
            });

            ruleDb.Add(ruleId, new DocRuleDto() {
                DocId = ruleId,
                Name = $"Doc-{i}",
                StatusCode = 200,
                Body = $"some json {i}",
                DelayMsec = 50,
                Conditions = new []{ new WhenDto(){ BooleanExpression = "1 == 1", IsActive = true}}
            });

        }
    }
}
