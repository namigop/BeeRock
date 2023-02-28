using LanguageExt;

namespace BeeRock.Core.UseCases.SaveServiceDetails;

public interface ISaveServiceDetailsUseCase {
    TryAsync<Unit> Save(string docId, string serviceName, int port, string swagger,bool isDynamic);
}
