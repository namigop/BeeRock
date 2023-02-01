using BeeRock.Core.Interfaces;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI;

public class RestRequestArgsUIProvider : IRestRequestTestArgsProvider {
    public IRestRequestTestArgs Find(string methodName) {
        // var m = Global.CurrentServices
        //     .Where(c => c is TabItemService)
        //     .Cast<TabItemService>()
        //     .SelectMany(c => c.Methods)
        //     .First(t => t.Method.MethodName == methodName);

        foreach (var svc in Global.CurrentServices.Where(c => c is TabItemService).Cast<TabItemService>()) {
            //var swaggerUrl = c.Settings.SourceSwaggerDoc;
            var swaggerUrl = svc.SwaggerUrl; //
            foreach (var m in svc.Methods)
                if (m.Method.MethodName == methodName) {
                    svc.LoadSelecteMethod(m).ConfigureAwait(false).GetAwaiter().GetResult();
                    return new RestRequestTestArgs(m, swaggerUrl);
                }
        }

        throw new Exception($"Method {methodName} was not found");
    }
}
