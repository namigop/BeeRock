using System.Text;
using System.Text.RegularExpressions;
using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities.CodeGen;

public class MethodModifier : ICodeModifier {
    private const string MethodFragment = "public System.Threading.Tasks.Task";
    private const string MethodRegex = @"\s+System.Threading.Tasks.Task.*\s(?<MethodName>\w+)\(.*\)";
    private const string MethodRegexWithReturnValue = @"\s+System.Threading.Tasks.Task\<(?<EntityName>.+)\>\s+\w+\(.*\)";
    private const string ReturnFragment = "return _implementation.";
    private readonly StringBuilder _code;
    private readonly string _controllerName;

    public MethodModifier(StringBuilder code, string controllerName) {
        _code = code;
        _controllerName = controllerName;
    }

    public StringBuilder Modify() {
        var reader = new StringReader(_code.ToString());
        var newBuilder = new StringBuilder();

        var currentEntity = "";
        var currentMethod = "";
        while (reader.ReadLine() is { } line) {
            if (line.Contains(MethodFragment)) {
                var (a, b) = ParseMethodLine(line);
                currentEntity = a;
                currentMethod = b;
            }

            if (line.Contains(ReturnFragment)) {
                var newImplementation = GetNewMethodImplementation(line, _controllerName, currentMethod, currentEntity);
                newBuilder.AppendLine(newImplementation);
                currentEntity = "";
                currentMethod = "";
            }
            else {
                newBuilder.AppendLine(line);
            }
        }

        return newBuilder;
    }

    private static string GetNewMethodImplementation(string line, string controllerName, string methodName, string entityName) {
        /* We want to replace this *example* line

            return _implementation.LoginUserAsync(username, password);

            and replace it with

            var p = BeeRock.Core.Mbduna040k5nControllerNS.RedirectCalls.CreateParameter(new string[] { "header", "username", "password" }, new object[] { this.Request.Headers, username, password });
            var json = BeeRock.Core.Mbduna040k5nControllerNS.RedirectCalls.HandleWithResponse("MGetUserByName462", p);
            return System.Threading.Tasks.Task.FromResult(Newtonsoft.Json.JsonConvert.DeserializeObject<User>(json));
        */

        static string WrapArgsInQoutes(string methodArgs) {
            var chunks = methodArgs.Split(",");
            return string.Join(',', chunks.Select(c => $"\"{c.Trim()}\""));
        }

        static string TryRemoveConditional(string arg) {
            if (string.IsNullOrWhiteSpace(arg)) return arg;

            //process args like this : request ?? "Foobar"
            var items = arg.Split(",");
            arg = items.Select(i => i.Split("??").First().Trim())
                .Then(all => string.Join(", ", all));
            return arg;
        }

        var sb = new StringBuilder();
        var start = line.IndexOf("(", StringComparison.InvariantCulture) + 1;
        var end = line.LastIndexOf(")", StringComparison.InvariantCulture);
        var arg = line.Substring(start, end - start).Then(TryRemoveConditional);

        var arrayArg = $"new object[] {{ this.HttpContext, {arg} }}";
        var stringArrayArg = $"new string[] {{ \"httpContext\", {WrapArgsInQoutes(arg)} }}";

        //if there are no method parameters
        if (string.IsNullOrWhiteSpace(arg)) {
            arrayArg = "new object[] { this.HttpContext }";
            stringArrayArg = "new string[] { \"httpContext\" }";
        }

        var createParamCode = $"var p = BeeRock.Core.{controllerName}NS.RedirectCalls.CreateParameter( {stringArrayArg}, {arrayArg});";
        sb.Append("            ");
        sb.AppendLine(createParamCode);

        if (string.IsNullOrEmpty(entityName)) {
            var newCode = $"var json = BeeRock.Core.{controllerName}NS.RedirectCalls.HandleWithResponse(\"{methodName}\", p);";
            sb.Append("            ");
            sb.AppendLine(newCode);
            sb.Append("            ");
            sb.AppendLine("return System.Threading.Tasks.Task.CompletedTask;");
        }
        else if (entityName.ToUpper().EndsWith("FILECONTENTRESULT")) {
            var newCode = $"var json = BeeRock.Core.{controllerName}NS.RedirectCalls.HandleWithFileResponse(\"{methodName}\", p);";
            sb.Append("            ");
            sb.AppendLine(newCode);
            sb.Append("            ");
            sb.AppendLine("return System.Threading.Tasks.Task.FromResult(json);");
        }
        else {
            var newCode = $"var json = BeeRock.Core.{controllerName}NS.RedirectCalls.HandleWithResponse(\"{methodName}\", p);";
            sb.Append("            ");
            sb.AppendLine(newCode);
            var conv = $"return System.Threading.Tasks.Task.FromResult(Newtonsoft.Json.JsonConvert.DeserializeObject<{entityName}>(json));";
            sb.Append("            ");
            sb.AppendLine(conv);
        }

        return sb.ToString();
    }

    private static (string, string) ParseMethodLine(string line) {
        var m = Regex.Match(line, MethodRegex);
        var currentMethod = m.Success ? m.Groups["MethodName"].Value : "";

        m = Regex.Match(line, MethodRegexWithReturnValue);
        var currentEntity = m.Success ? m.Groups["EntityName"].Value : "";
        return (currentEntity, currentMethod);
    }
}
