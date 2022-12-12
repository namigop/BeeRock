using BeeRock.Core.Entities;
using BeeRock.Ports.Repository;

namespace BeeRock.Ports.SaveServiceRulesUseCase;

public interface ISaveServiceRuleSetsUseCase {
    Task<string> Save(RestService  service);
}
