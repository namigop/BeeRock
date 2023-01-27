namespace BeeRock.Core.Interfaces;

public interface IRestRequestTestArgsProvider {
    IRestRequestTestArgs Find(string methodName);
}