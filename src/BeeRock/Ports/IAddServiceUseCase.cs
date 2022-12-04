using BeeRock.Core.Entities;
using LanguageExt;

namespace BeeRock.Ports;

public interface IAddServiceUseCase {
    TryAsync<RestService> AddService(AddServiceParams serviceParams);
}