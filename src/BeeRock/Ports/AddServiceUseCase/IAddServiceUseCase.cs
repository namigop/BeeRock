using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Ports.AddServiceUseCase;

public interface IAddServiceUseCase {
    TryAsync<IRestService> AddService(IAddServiceParams serviceParams);
}