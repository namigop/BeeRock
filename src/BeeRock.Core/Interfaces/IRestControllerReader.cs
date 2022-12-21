using BeeRock.Core.Entities;

namespace BeeRock.Core.Interfaces;

public interface IRestControllerReader {
    List<RestMethodInfo> Inspect(Type controllerType);
}
