using System.Text;
using BeeRock.Core.Utils;
using NSwag;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace BeeRock.Core.Entities.CodeGen;

public static class SwaggerCodeGen {
    public static async Task<string> GenerateControllers(string name, string swaggerJsonUrlOrFile) {
        swaggerJsonUrlOrFile = swaggerJsonUrlOrFile.Trim();
        var doc =
            swaggerJsonUrlOrFile.StartsWith("http")
                ? await OpenApiDocument.FromUrlAsync(swaggerJsonUrlOrFile)
                : await OpenApiDocument.FromFileAsync(swaggerJsonUrlOrFile);

        var g = new CSharpControllerGenerator(doc, new CSharpControllerGeneratorSettings {
            ControllerStyle = CSharpControllerStyle.Partial,
            RouteNamingStrategy = CSharpControllerRouteNamingStrategy.OperationId
        });

        //Generate the code using NSwag
        var code = g.GenerateFile(ClientGeneratorOutputType.Full);
        var sb = new StringBuilder(code);
        var controllerName = $"{name}Controller";

        //Modify the code because a lot of swagger docs are invalid and these results
        //in invalid generated code that does not compile
        return ModifyLines(sb)
            .Then(c => ModifyCode(c, controllerName))
            .ToString();
    }

    public static Type GenerateClient(string swaggerDoc) {
        var doc = OpenApiDocument.FromUrlAsync(swaggerDoc).ConfigureAwait(false).GetAwaiter().GetResult();
        var className = ScriptingVarProxy.BeeRockClient;
        var clientFile = $"{className}-gen.cs";
        var clientDll = $"{className}-gen.dll";

        var clientSettings = new CSharpClientGeneratorSettings {
            UseBaseUrl = false,
            ClassName = className,
            DisposeHttpClient = true,
            InjectHttpClient = false
        };

        clientSettings.OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator();
        var client = new CSharpClientGenerator(doc, clientSettings);
        var clientCode = client.GenerateFile(ClientGeneratorOutputType.Full);
        clientCode += GeneratePartialClient();

#if DEBUG
        File.WriteAllText(Path.Combine(Helper.GetTempPath(), clientFile), clientCode);
#endif

        var compiler = new CsCompiler(clientDll, clientCode);
        compiler.Compile();
        if (!compiler.Success) {
            var allErrors = new StringBuilder();
            foreach (var error in compiler.CompilationErrors) allErrors.AppendLine(error);

            throw new Exception($"Unable to generate the proxy client.{Environment.NewLine} {allErrors} ");
        }

        var clientType = compiler.GetTypes().FirstOrDefault(t => t.Name == className);
        return clientType;
    }

    private static string GeneratePartialClient() {
        //BeeRock.Core.Entities.ScriptingVarProxy.PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);
        var code = @"

namespace MyNamespace
{
    public partial class BeeRockClient
    {

        static System.Reflection.MethodInfo prepareRequest = System.Reflection.Assembly
                .GetEntryAssembly()
                .GetType(""BeeRock.Program"")
                .GetMethod(""GetProxyRequestProcessor"")
                .Invoke(null,null) as System.Reflection.MethodInfo;

        static System.Reflection.MethodInfo processResponse = System.Reflection.Assembly
                .GetEntryAssembly()
                .GetType(""BeeRock.Program"")
                .GetMethod(""GetProxyResponseProcessor"")
                .Invoke(null,null) as System.Reflection.MethodInfo;

        public Microsoft.AspNetCore.Http.IHeaderDictionary Headers { get; set;}

        public string TargetUrl { get; set;}

        public bool IsForwardingToFullUrl { get; set;}

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
             prepareRequest.Invoke(null, new object[]{ client, request, url, this.Headers, this.TargetUrl, this.IsForwardingToFullUrl });
        }
        
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
        {
             processResponse.Invoke(null, new object[]{ client, response });
        }
    }
}

";

        return code;
    }

    /// <summary>
    ///     Modify the generated server code
    /// </summary>
    private static StringBuilder ModifyCode(StringBuilder code, string controllerName) {
        var m = new MethodModifier(code, controllerName);
        code = m.Modify()
            .Then(c => new AddRedirectClassModifier(c, controllerName))
            .Modify();
        return code;
    }

    private static StringBuilder ModifyLines(StringBuilder code) {
        var lineModifiers = new List<ILineModifier> {
            new CollectionModifier(),
            new DictionaryModifier(),
            new FileResultModifier(),
            new ControllerClassNameModifier(),
            new ConstructorInitModifier(),
            new ConstructorLineModifier(),
            new MethodLineModifier(),
            new RouteDoubleSlashModifier()
        };

        var sb = new StringBuilder();

        using var reader = new StringReader(code.ToString());
        var lineNumber = 0;
        while (reader.ReadLine() is { } line) {
            lineNumber += 1;
            foreach (var m in lineModifiers)
                if (m.CanModify(line, lineNumber))
                    line = m.Modify();

            sb.AppendLine(line);
        }

        return sb;
    }
}
