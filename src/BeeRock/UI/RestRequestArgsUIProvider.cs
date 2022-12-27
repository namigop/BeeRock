using BeeRock.Core.Interfaces;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI;

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
