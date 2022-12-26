using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.StartService;

public interface IStartServiceUseCase {
    TryAsync<bool> Start(IRestService service);
}