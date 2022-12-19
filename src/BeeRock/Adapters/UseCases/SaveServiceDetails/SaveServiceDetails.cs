using BeeRock.Ports;
using BeeRock.Ports.Repository;
using BeeRock.Ports.SaveServiceDetailsUseCase;

namespace BeeRock.Adapters.UseCases.SaveServiceDetails;

public class SaveServiceDetailsUseCase : UseCaseBase, ISaveServiceDetailsUseCase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public SaveServiceDetailsUseCase(IDocServiceRuleSetsRepo svcRepo) {
        _svcRepo = svcRepo;
    }

    public async Task Save(string docId, string serviceName, int port, string swagger) {
        var dao = await _svcRepo.Read(docId);
        dao.PortNumber = port;
        dao.ServiceName = serviceName;
        dao.SourceSwagger = swagger;
        await _svcRepo.Update(dao);
    }
}
