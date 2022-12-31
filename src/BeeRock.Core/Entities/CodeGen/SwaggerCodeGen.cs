using System.Text;
using BeeRock.Core.Utils;
using NSwag;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;

namespace BeeRock.Core.Entities.CodeGen;

public static class SwaggerCodeGen {
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
}
