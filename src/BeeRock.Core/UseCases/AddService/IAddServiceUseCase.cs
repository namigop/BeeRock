using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.AddService;

public interface IAddServiceUseCase {
    TryAsync<ICompiledRestService> AddService(AddServiceParams serviceParams);
}
