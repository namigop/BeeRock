using BeeRock.Core.Entities;
using LanguageExt;
using LanguageExt.Common;

namespace BeeRock.Ports;

public interface IStartServiceUseCase {
    TryAsync<bool> Start(RestService service);
}
