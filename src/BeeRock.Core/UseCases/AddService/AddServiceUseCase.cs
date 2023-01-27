using System.Diagnostics;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.CodeGen;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;
using LanguageExt.Common;

namespace BeeRock.Core.UseCases.AddService;

public class AddServiceUseCase : UseCaseBase, IAddServiceUseCase {
    private readonly Func<string, string, ICsCompiler> _compilerBuilder;
    private readonly Func<string, string, Task<string>> _generateCode;
    private readonly Func<Type[], string, RestServiceSettings, IRestService> _svcBuilder;

    public AddServiceUseCase(
        Func<string, string, Task<string>> generateCode,
        Func<string, string, ICsCompiler> compilerBuilder,
        Func<Type[], string, RestServiceSettings, IRestService> svcBuilder) {
        _compilerBuilder = compilerBuilder;
        _svcBuilder = svcBuilder;
        _generateCode = generateCode;
    }

    public AddServiceUseCase() : this(
        SwaggerCodeGen.GenerateControllers,
        (dll, code) => new CsCompiler(dll, code),
        (types, name, settings) => new RestService(types, name, settings)) {
    }

    public bool IsBusy { get; set; }

    /// <summary>
    ///     Generate a service based on a json swagger doc
    /// </summary>
    public TryAsync<IRestService> AddService(AddServiceParams serviceParams) {
        var rand = $"M{Path.GetRandomFileName().Replace(".", "")}";
        var csFile = Path.Combine(serviceParams.TempPath, $"BeeRock-Controller{rand}-gen.cs");
        var dll = Path.Combine(serviceParams.TempPath, csFile.Replace(".cs", ".dll"));

        return
            GenerateCode(serviceParams, rand)
                .Bind(csCode => Compile(csCode, csFile, dll))
                .Bind(GetControllerTypes)
                .Bind(controllerTypes => CreateRestService(serviceParams, controllerTypes));
    }

    [Conditional("DEBUG")]
    private static void Dump(string csFile, string csCode, ICsCompiler compiler) {
        File.WriteAllText(csFile, csCode);
        if (compiler.CompilationErrors.Any()) File.WriteAllLines(csFile + ".compile-error.txt", compiler.CompilationErrors.ToArray());
    }

    /// <summary>
    ///     Get the controller types that implements a rest service
    /// </summary>
    private static TryAsync<Type[]> GetControllerTypes(ICsCompiler compiler) {
        return async () => {
            var val = Requires.NotNull2<Type[]>(compiler, nameof(compiler));
            if (val.IsFaulted)
                return val;

            var controllerTypes = compiler.GetTypes()
                .Where(t => t.Name.EndsWith("Controller") && t.IsClass)
                .ToArray();

            if (controllerTypes.Length == 0) {
                var exc = new Exception("Unable to generate service controllers");
                return new Result<Type[]>(exc);
            }

            await Task.Yield();
            return controllerTypes;
        };
    }

    /// <summary>
    ///     Compile the generated C# server code
    /// </summary>
    private TryAsync<ICsCompiler> Compile(string csCode, string csFile, string dll) {
        Requires.NotNullOrEmpty(csCode, nameof(csCode));
        Requires.NotNullOrEmpty(dll, nameof(dll));
        C.Info($"Compiling to {dll}");

        return async () => {
            Info("Compiling...");
            var compiler = await Task.Run(() => {
                var compiler = _compilerBuilder(dll, csCode);
                compiler.Compile();
                return compiler;
            });

            Dump(csFile, csCode, compiler);
            if (compiler.CompilationErrors.Any()) {
                Error("Compilation failed!");
                var exc = new Exception(string.Join(Environment.NewLine, compiler.CompilationErrors));
                throw exc;
            }

            return new Result<ICsCompiler>(compiler);
        };
    }

    private TryAsync<IRestService> CreateRestService(AddServiceParams serviceParams, Type[] controllerTypes) {
        C.Info($"Inspecting server code. Found {controllerTypes.Length} controller types");

        return async () => {
            var val = Requires.NotNull2<IRestService>(serviceParams, nameof(serviceParams))
                .Bind(() => Requires.NotNullOrEmpty2<Type, IRestService>(controllerTypes, nameof(controllerTypes)));
            if (val.IsFaulted)
                return val;

            var name = !string.IsNullOrWhiteSpace(serviceParams.ServiceName) ? serviceParams.ServiceName : "My Service";
            var settings = new RestServiceSettings { Enabled = true, PortNumber = serviceParams.Port, SourceSwaggerDoc = serviceParams.SwaggerUrl };
            var restService = _svcBuilder(controllerTypes, name, settings);
            restService.DocId = serviceParams.DocId;

            await Task.Yield();
            return new Result<IRestService>(restService);
        };
    }

    /// <summary>
    ///     Generate C# rest server code
    /// </summary>
    private TryAsync<string> GenerateCode(AddServiceParams serviceParams, string rand) {
        Requires.NotNullOrEmpty(serviceParams.SwaggerUrl, nameof(serviceParams.SwaggerUrl));
        C.Info($"Generating server code for {serviceParams.SwaggerUrl}");
        return async () => {
            var code = await _generateCode(rand, serviceParams.SwaggerUrl);
            return code;
        };
    }
}
