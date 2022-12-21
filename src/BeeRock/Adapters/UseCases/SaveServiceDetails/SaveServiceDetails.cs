using BeeRock.Core.Ports;
using BeeRock.Core.Ports.SaveServiceDetailsUseCase;
using BeeRock.Ports.Repository;
using LanguageExt;

namespace BeeRock.Adapters.UseCases.SaveServiceDetails;

public class SaveServiceDetailsUseCase : UseCaseBase, ISaveServiceDetailsUseCase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public SaveServiceDetailsUseCase(IDocServiceRuleSetsRepo svcRepo) {
        _svcRepo = svcRepo;
    }

    public TryAsync<Unit> Save(string docId, string serviceName, int port, string swagger) {
        return async () => {
            var dao = await Task.Run(() => _svcRepo.Read(docId));
            dao.PortNumber = port;
            dao.ServiceName = serviceName;
            dao.SourceSwagger = swagger;
            await Task.Run(() => _svcRepo.Update(dao));
            return Unit.Default;
        };
    }
}
