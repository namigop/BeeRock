using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Interfaces;

namespace BeeRock.Adapters.UI;

public class RestRequestArgsUIProvider : IRestRequestTestArgsProvider {
    public IRestRequestTestArgs Find(string methodName) {
        var m = Global.CurrentServices
            .Where(c => c is TabItemService)
            .Cast<TabItemService>()
            .SelectMany(c => c.Methods)
            .First(t => t.Method.MethodName == methodName);
        return new RestRequestTestArgs(m);
    }
}
