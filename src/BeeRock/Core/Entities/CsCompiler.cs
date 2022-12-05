using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace BeeRock.Core.Entities;

public class CsCompiler {
    public CsCompiler(OutputKind targetOutput, string assemblyFile, params string[] sourceFiles) {
        ModuleFile = assemblyFile;
        SourceFiles = sourceFiles;

        TargetOutput = targetOutput;
        CompilationErrors = new List<string>();
    }

    public CsCompiler(string assemblyFile, params string[] sourceFiles) : this(OutputKind.DynamicallyLinkedLibrary,
        assemblyFile, sourceFiles) {
    }

    public List<string> CompilationErrors { get; }
    public byte[] CompiledBytes { get; private set; }
    public string ModuleFile { get; }
    public string ModuleName => Path.GetFileName(ModuleFile);
    public string[] SourceFiles { get; }
    public bool Success { get; private set; }
    public OutputKind TargetOutput { get; }

    public void Compile(params MetadataReference[] additionalReferences) {
        CompiledBytes = null;
        CompilationErrors.Clear();
        Success = false;

        using (var ms = new MemoryStream()) {
            var compilation = CreateCompilation(SourceFiles, ModuleName, TargetOutput, additionalReferences);

            var result = compilation.Emit(ms);
            Success = result.Success;
            if (result.Success) {
                ms.Seek(0, SeekOrigin.Begin);
                CompiledBytes = ms.ToArray();
            }
            else {
                var failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                foreach (var diagnostic in failures)
                    CompilationErrors.Add($"{diagnostic.Id}, {diagnostic.GetMessage()}");
            }
        }
    }

    public Type[] GetTypes() {
        using (var ms = new MemoryStream(CompiledBytes)) {
            _ = ms.Seek(0, SeekOrigin.Begin);
            var context = new SimpleAssemblyLoadContext();
            var assembly = context.LoadFromStream(ms);

            var f = assembly.GetTypes();
            return f;
        }
    }

    public Task<string> Save() {
        Directory.CreateDirectory(Path.GetDirectoryName(ModuleFile));
        File.WriteAllBytes(ModuleFile, CompiledBytes);
        return Task.FromResult(ModuleFile);
    }

    protected List<string> GetOtherReferencedFiles() {
        return new List<string>();
    }

    private static List<string> GetInternalReferencedFiles() {
        var referencedFiles = new List<string>();
        // referencedFiles.Add(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location);

        var fullName = typeof(CSharpArgumentInfo).Assembly.FullName;
        var dll = fullName.Split(",").First() + ".dll";
        referencedFiles.Add(Path.Combine(AppContext.BaseDirectory, dll));
        foreach (var fullPath in
                 ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)) {
            var fileName = Path.GetFileName(fullPath);
            Debug.WriteLine(fileName);
            if (fileName.StartsWith("System") ||
                fileName.StartsWith("mscorlib") ||
                fileName.StartsWith("netstandard") ||
                fileName.StartsWith("Microsoft.") ||
                fileName.StartsWith("Swashbuckle.") ||
                fileName.StartsWith("Newtonsoft."))
                referencedFiles.Add(fullPath);
        }

        return referencedFiles;
    }

    private CSharpCompilation CreateCompilation(string[] sourceFiles, string assemblyOrModuleName,
        OutputKind outputKind, params MetadataReference[] additionalReferences) {
        var syntaxTrees = sourceFiles.Select(s => {
            var codeString = SourceText.From(File.ReadAllText(s));
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);
            return parsedSyntaxTree;
        });

        var referencedFiles = GetInternalReferencedFiles();
        var grpcFiles = GetOtherReferencedFiles();
        referencedFiles.AddRange(grpcFiles);
        var references =
            referencedFiles
                .Where(f => File.Exists(f))
                .Select(refFile => MetadataReference.CreateFromFile(refFile))
                .Concat(additionalReferences)
                .ToArray();

        return CSharpCompilation.Create(assemblyOrModuleName,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(outputKind,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }
}
