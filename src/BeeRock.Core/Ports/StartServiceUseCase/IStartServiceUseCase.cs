using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.Ports.StartServiceUseCase;

public interface IStartServiceUseCase {
    TryAsync<bool> Start(IRestService service);
}