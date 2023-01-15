using Microsoft.CodeAnalysis;

namespace BeeRock.Core.Interfaces;

public interface ICsCompiler {
    List<string> CompilationErrors { get; }
    string[] SourceCodeStrings { get; }
    bool Success { get; }
    OutputKind TargetOutput { get; }

    void Compile(params MetadataReference[] additionalReferences);

    Type[] GetTypes();
}