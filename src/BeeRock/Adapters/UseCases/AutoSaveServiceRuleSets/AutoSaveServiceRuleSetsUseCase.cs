using BeeRock.Adapters.UseCases.SaveServiceRuleSets;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.AutoSaveServiceRuleSetsUseCase;
using BeeRock.Ports.Repository;

namespace BeeRock.Adapters.UseCases.AutoSaveServiceRuleSets;

public class AutoSaveServiceRuleSetsUseCase : UseCaseBase, IAutoSaveServiceRuleSetsUseCase {
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private bool canSave = true;

    public AutoSaveServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
    }

    public async Task Start(Func<IRestService> getService) {
        var uc = new SaveServiceRuleSetsUseCase(_svcRepo, _ruleRepo);

        while (canSave) {
            Info($"INFO: {DateTime.Now} : BeeRock: Auto-save started.");
            var svc = getService();
            if (svc != null) svc.DocId = await uc.Save(svc);

            await Task.Delay(TimeSpan.FromSeconds(20));
        }
    }

    public Task Stop() {
        canSave = false;
        return Task.CompletedTask;
    }
}
