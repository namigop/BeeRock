using BeeRock.Core.Interfaces;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI;

public class RestRequestArgsUIProvider : IRestRequestTestArgsProvider {
    private static object key = new();
    public IRestRequestTestArgs Find(string methodName, int port) {
        foreach (var svc in Global.CurrentServices.Where(c => c is TabItemService).Cast<TabItemService>()) {
            //var swaggerUrl = c.Settings.SourceSwaggerDoc;
            var swaggerUrl = svc.SwaggerUrl; //
            foreach (var m in svc.Methods)
                if (m.Method.MethodName == methodName && svc.Settings.PortNumber == port) {
                    lock (key) {
                        svc.LoadSelecteMethod(m).ConfigureAwait(false).GetAwaiter().GetResult();
                    }

                    return new RestRequestTestArgs(m, swaggerUrl);
                }
        }

        throw new Exception($"Method {methodName} was not found");
    }
}
