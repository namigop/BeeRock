using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Ports.StartServiceUseCase;

public interface IStartServiceUseCase {
    TryAsync<bool> Start(IRestService service);
}