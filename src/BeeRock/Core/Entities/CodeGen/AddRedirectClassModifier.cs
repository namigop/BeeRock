using System.Text;

namespace BeeRock.Core.Entities.CodeGen;

public class AddRedirectClassModifier : ICodeModifier {
    private const string Redir = @"

namespace BeeRock.Core.{{.ControllerName}}NS
{

	public static class RedirectCalls
	{
		static System.Reflection.MethodInfo method= System.Reflection.Assembly.GetEntryAssembly().GetType(""BeeRock.Core.Entities.RequestHandler"").GetMethod(""Handle"");


        public static System.Collections.Generic.Dictionary<string, object> CreateParameter(string[] keys, object[] values) {
            var dict = new System.Collections.Generic.Dictionary<string, object>();
            for (int i = 0; i < keys.Length; i++) {
                dict.Add(keys[i], values[i]);
            }

            return dict;
        }

		 public static string HandleWithResponse(string methodName, System.Collections.Generic.Dictionary<string, object> parameters) {
           
       	    var r = method.Invoke(null, new object[] {methodName, parameters} );
            return r != null ? r.ToString() :  """";
			 
		}
	}
}

";

    private readonly StringBuilder _code;
    private readonly string _controllerName;

    public AddRedirectClassModifier(StringBuilder code, string controllerName) {
        _code = code;
        _controllerName = controllerName;
    }

    public StringBuilder Modify() {
        _code.AppendLine();
        return _code.AppendLine(Redir.Replace("{{.ControllerName}}", _controllerName));
    }
}
