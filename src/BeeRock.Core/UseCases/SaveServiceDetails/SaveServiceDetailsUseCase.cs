using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;
using LanguageExt.Common;

namespace BeeRock.Core.UseCases.SaveServiceDetails;

public class SaveServiceDetailsUseCase : UseCaseBase, ISaveServiceDetailsUseCase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public SaveServiceDetailsUseCase(IDocServiceRuleSetsRepo svcRepo) {
        _svcRepo = svcRepo;
    }

    /// <summary>
    ///     Save a service to the DB.  It will be assigned a docId (GUID) if it is new
    /// </summary>
    public TryAsync<Unit> Save(string svcDocId, string serviceName, int port, string swagger, bool isDynamic) {
        return async () => {
            var res = Requires.NotNullOrEmpty2<Unit>(svcDocId, nameof(svcDocId))
                .Bind(() => Requires.NotNullOrEmpty2<Unit>(serviceName, nameof(serviceName)))
                .Bind(() => !isDynamic ? Requires.NotNullOrEmpty2<Unit>(swagger, nameof(swagger)) : new Result<Unit>(default(Unit)))
                .Bind(() => Requires.IsTrue2<Unit>(() => port > 0, nameof(serviceName)));

            if (res.IsFaulted)
                return res;

            C.Debug($"Saving service with ID {svcDocId}");
            var dto = await Task.Run(() => _svcRepo.Read(svcDocId));
            dto.PortNumber = port;
            dto.ServiceName = serviceName;
            dto.SourceSwagger = swagger;
            dto.LastUpdated = DateTime.Now;
            await Task.Run(() => _svcRepo.Update(dto));
            return Unit.Default;
        };
    }
}
