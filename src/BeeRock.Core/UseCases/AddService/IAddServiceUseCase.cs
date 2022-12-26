using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.AddService;

public interface IAddServiceUseCase {
    TryAsync<IRestService> AddService(AddServiceParams serviceParams);
}
