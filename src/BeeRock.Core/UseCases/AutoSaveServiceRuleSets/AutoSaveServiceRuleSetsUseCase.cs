using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.SaveServiceRuleSets;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.AutoSaveServiceRuleSets;

public class AutoSaveServiceRuleSetsUseCase : UseCaseBase, IAutoSaveServiceRuleSetsUseCase {
    private const int SaveInterval = 10; //sec
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private bool canSave = true;

    public AutoSaveServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
    }

    /// <summary>
    ///     Start the auto save loop defined by the save interval
    /// </summary>
    public TryAsync<Unit> Start(Func<IRestService> getService) {
        return async () => {
            var uc = new SaveServiceRuleSetsUseCase(_svcRepo, _ruleRepo);

            while (canSave) {
                C.Info("Auto-save started");

                var svc = getService();
                if (svc != null) {
                    await uc.Save(svc).IfSucc(id => svc.DocId = id);
                }

                await Task.Delay(TimeSpan.FromSeconds(SaveInterval));
            }

            return new Unit();
        };
    }

    /// <summary>
    ///     Stop the auto save loop
    /// </summary>
    public TryAsync<Unit> Stop() {
        return async () => {
            canSave = false;
            await Task.Yield(); //just to clear the async warning
            return new Unit();
        };
    }
}
