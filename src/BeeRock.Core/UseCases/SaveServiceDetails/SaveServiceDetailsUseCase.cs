using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.SaveServiceDetails;

public class SaveServiceDetailsUseCase : UseCaseBase, ISaveServiceDetailsUseCase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public SaveServiceDetailsUseCase(IDocServiceRuleSetsRepo svcRepo) {
        _svcRepo = svcRepo;
    }

    /// <summary>
    ///     Save a service to the DB.  It will be assigned a docId (GUID) if it is new
    /// </summary>
    public TryAsync<Unit> Save(string svcDocId, string serviceName, int port, string swagger) {
        return async () => {
            var res = Requires.NotNullOrEmpty2<Unit>(svcDocId, nameof(svcDocId))
                .Bind(() => Requires.NotNullOrEmpty2<Unit>(serviceName, nameof(serviceName)))
                .Bind(() => Requires.NotNullOrEmpty2<Unit>(swagger, nameof(swagger)))
                .Bind(() => Requires.IsTrue2<Unit>(() => port > 0, nameof(serviceName)));

            if (res.IsFaulted)
                return res;

            C.Debug($"Saving service with ID {svcDocId}");
            var dao = await Task.Run(() => _svcRepo.Read(svcDocId));
            dao.PortNumber = port;
            dao.ServiceName = serviceName;
            dao.SourceSwagger = swagger;
            dao.LastUpdated = DateTime.Now;
            await Task.Run(() => _svcRepo.Update(dao));
            return Unit.Default;
        };
    }
}