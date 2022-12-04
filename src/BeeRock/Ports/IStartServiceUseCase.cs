using BeeRock.Core.Entities;
using LanguageExt;

namespace BeeRock.Ports;

public interface IStartServiceUseCase {
    TryAsync<bool> Start(RestService service);
}