using BeeRock.Core.Interfaces;
using Microsoft.CodeAnalysis;

namespace BeeRock.Tests.UseCases.Fakes;

public class FakeCsCompiler : ICsCompiler {
    public Type[] GetTypes() {
        return new[] { typeof(FakeController) };
    }

    public List<string> CompilationErrors { get; } = new();
    public string[] SourceCodeStrings { get; } = Array.Empty<string>();
    public bool Success { get; } = true;
    public OutputKind TargetOutput { get; } = OutputKind.DynamicallyLinkedLibrary;

    public void Compile(params MetadataReference[] additionalReferences) {
        //do nothing. We already have the TestController Type
    }
}
