using System.Diagnostics;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.CodeGen;
using BeeRock.Core.Utils;
using BeeRock.Ports;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.CodeAnalysis;

namespace BeeRock.Adapters.UseCases.AddService;

public class AddServiceUseCase : UseCaseBase, IAddServiceUseCase {
    public bool IsBusy { get; set; }

    public TryAsync<RestService> AddService(AddServiceParams serviceParams) {
        var rand = $"M{Path.GetRandomFileName().Replace(".", "")}";
        var fileName = $"BeeRock-Controller{rand}-gen.cs";
        var dll = fileName.Replace(".cs", ".dll");

        return GenerateCode(serviceParams, rand, fileName)
            .Bind(csFile => Compile(csFile, dll, this))
            .Bind(GetControllerTypes)
            .Bind(controllerTypes => CreateRestService(serviceParams, controllerTypes));
    }

    [Conditional("DEBUG")]
    private static void Dump(string csFile, CsCompiler compiler) {
        File.WriteAllLines(csFile + ".compile-error.txt", compiler.CompilationErrors.ToArray());
    }

    private static TryAsync<CsCompiler> Compile(string csFile, string dll, AddServiceUseCase self) {
        return async () => {
            self.Info("Compiling...");
            var compiler = await Task.Run(() => {
                var compiler = new CsCompiler(OutputKind.DynamicallyLinkedLibrary, dll, csFile);
                compiler.Compile();
                return compiler;
            });

            if (compiler.CompilationErrors.Any()) {
                self.Error("Compilation failed!");
                Dump(csFile, compiler);
                var exc = new Exception(string.Join(Environment.NewLine, compiler.CompilationErrors));
                throw exc;
            }

            return compiler;
        };
    }

    private static TryAsync<Type[]> GetControllerTypes(CsCompiler compiler) {
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

    private static TryAsync<RestService> CreateRestService(AddServiceParams serviceParams, Type[] controllerTypes) {
        return () => {
            var restService = new RestService(controllerTypes) {
                Name = !string.IsNullOrWhiteSpace(serviceParams.ServiceName) ? serviceParams.ServiceName : "My Service",
                Settings = new RestServiceSettings { Enabled = true, PortNumber = serviceParams.Port }
            };
            return Task.FromResult<Result<RestService>>(restService);
        };
    }

    private static TryAsync<string> GenerateCode(AddServiceParams serviceParams, string rand, string fileName) {
        return async () => {
            var code = await SwaggerCodeGen.GenerateControllers(rand, serviceParams.SwaggerUrl);
            var csFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .Then(p => Path.Combine(p, "BeeRock"))
                .Then(dir => {
                    Directory.CreateDirectory(dir);
                    return Path.Combine(dir, fileName);
                });

            await File.WriteAllTextAsync(csFile, code);
            return csFile;
        };
    }
}