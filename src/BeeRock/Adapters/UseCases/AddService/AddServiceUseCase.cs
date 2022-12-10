using System.Diagnostics;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using BeeRock.Ports;
using BeeRock.Ports.AddServiceUseCase;
using LanguageExt;
using LanguageExt.Common;

namespace BeeRock.Adapters.UseCases.AddService;

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

    public bool IsBusy { get; set; }

    public TryAsync<IRestService> AddService(IAddServiceParams serviceParams) {
        var rand = $"M{Path.GetRandomFileName().Replace(".", "")}";
        var fileName = $"BeeRock-Controller{rand}-gen.cs";
        var dll = fileName.Replace(".cs", ".dll");

        return
            GenerateCode(serviceParams, rand)
                .Bind(csCode => Compile(csCode, dll))
                .Bind(GetControllerTypes)
                .Bind(controllerTypes => CreateRestService(serviceParams, controllerTypes));
    }

    [Conditional("DEBUG")]
    private static void Dump(string csFile, ICsCompiler compiler) {
        File.WriteAllLines(csFile + ".compile-error.txt", compiler.CompilationErrors.ToArray());
    }

    private TryAsync<ICsCompiler> Compile(string csCode, string dll) {
        Requires.NotNullOrEmpty(csCode, nameof(csCode));
        Requires.NotNullOrEmpty(dll, nameof(dll));

        return async () => {
            Info("Compiling...");
            var compiler = await Task.Run(() => {
                var compiler = _compilerBuilder(dll, csCode);
                compiler.Compile();
                return compiler;
            });

            if (compiler.CompilationErrors.Any()) {
                Error("Compilation failed!");
                Dump(dll.Replace(".dll", ""), compiler);
                var exc = new Exception(string.Join(Environment.NewLine, compiler.CompilationErrors));
                throw exc;
            }

            return new Result<ICsCompiler>(compiler);
        };
    }

    private static TryAsync<Type[]> GetControllerTypes(ICsCompiler compiler) {
        Requires.NotNull(compiler, nameof(compiler));

        return () => {
            var controllerTypes = compiler.GetTypes()
                .Where(t => t.Name.EndsWith("Controller") && t.IsClass)
                .ToArray();

            if (controllerTypes.Length == 0) {
                var exc = new Exception("Unable to generate service controllers");
                throw exc;
            }

            return Task.FromResult<Result<Type[]>>(controllerTypes);
        };
    }

    private TryAsync<IRestService> CreateRestService(IAddServiceParams serviceParams, Type[] controllerTypes) {
        Requires.NotNull(serviceParams, nameof(serviceParams));
        Requires.NotNullOrEmpty(controllerTypes, nameof(controllerTypes));

        return () => {
            var name = !string.IsNullOrWhiteSpace(serviceParams.ServiceName) ? serviceParams.ServiceName : "My Service";
            var settings = new RestServiceSettings { Enabled = true, PortNumber = serviceParams.Port };
            var restService = _svcBuilder(controllerTypes, name, settings);
            return Task.FromResult(new Result<IRestService>(restService));
        };
    }

    private TryAsync<string> GenerateCode(IAddServiceParams serviceParams, string rand) {
        Requires.NotNullOrEmpty(serviceParams.SwaggerUrl, nameof(serviceParams.SwaggerUrl));

        return async () => {
            var code = await _generateCode(rand, serviceParams.SwaggerUrl);
            return code;
        };
    }
}