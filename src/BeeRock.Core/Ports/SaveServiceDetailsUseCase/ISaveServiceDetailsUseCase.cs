using LanguageExt;

namespace BeeRock.Core.Ports.SaveServiceDetailsUseCase;

public interface ISaveServiceDetailsUseCase {
    TryAsync<Unit> Save(string docId, string serviceName, int port, string swagger);
}
