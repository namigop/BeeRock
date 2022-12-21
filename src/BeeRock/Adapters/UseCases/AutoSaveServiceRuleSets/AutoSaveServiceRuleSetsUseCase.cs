using BeeRock.Adapters.UseCases.SaveServiceRuleSets;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.AutoSaveServiceRuleSetsUseCase;
using BeeRock.Ports.Repository;
using LanguageExt;

namespace BeeRock.Adapters.UseCases.AutoSaveServiceRuleSets;

public class AutoSaveServiceRuleSetsUseCase : UseCaseBase, IAutoSaveServiceRuleSetsUseCase {
    private const int SaveInterval = 15; //sec
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private bool canSave = true;

    public AutoSaveServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
    }

    public TryAsync<Unit> Start(Func<IRestService> getService) {
        return async () => {
            var uc = new SaveServiceRuleSetsUseCase(_svcRepo, _ruleRepo);

            while (canSave) {
                Info($"INFO: {DateTime.Now} : BeeRock: Auto-save started.");
                var svc = getService();
                if (svc != null) await uc.Save(svc).IfSucc(id => svc.DocId = id);

                await Task.Delay(TimeSpan.FromSeconds(SaveInterval));
            }

            return new Unit();
        };
    }

    public TryAsync<Unit> Stop() {
        return async () => {
            canSave = false;
            await Task.Yield(); //just to clear the async warning
            return new Unit();
        };
    }
}
