using System.Diagnostics;
using System.Reactive;
using BeeRock.Adapters.UI.Models;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.CodeGen;
using Microsoft.CodeAnalysis;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private string _addServiceLogMessage = "ready...";
    private string _name;

    private int _portNumber = 8000;
    private ServiceItem _service;
    public ReactiveCommand<Unit, Unit> AddCommand => ReactiveCommand.Create(OnAdd);

    public string AddServiceLogMessage {
        get => _addServiceLogMessage;
        set => this.RaiseAndSetIfChanged(ref _addServiceLogMessage, value);
    }

    public ServiceItem Service {
        get => _service;
        set => this.RaiseAndSetIfChanged(ref _service, value);
    }

    public int PortNumber {
        get => _portNumber;
        set => this.RaiseAndSetIfChanged(ref _portNumber, value);
    }

    public string Name {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private async void OnAdd() {
        // Swagger Docs
        // https://qcl-inventory.cxos.tech/swagger/doc.json IMS
        // /Users/erik.araojo/Downloads/sandbox-proxy-new.json
        var file = "BeeRock-Controller-gen.cs";
        var dll = "BeeRock-Controller-Gen.dll";

        try {
            AddServiceLogMessage = "Generating server code from swagger doc";
            var code = await SwaggerCodeGen.GenerateControllers("PetStore", SwaggerUrl);
            var currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            file = Path.Combine(currentDir, file);
            File.WriteAllText(file, code);

            AddServiceLogMessage = "Compiling...";
            var compiler = new CsCompiler(OutputKind.DynamicallyLinkedLibrary, dll, file);
            await Task.Run(() => compiler.Compile());

            if (compiler.CompilationErrors.Any()) {
                AddServiceLogMessage = "Compilation failed!";
                File.WriteAllLines(file + ".compile-error.txt", compiler.CompilationErrors.ToArray());
                Debugger.Break();
            }
            else {
                var controllerTypes =
                    compiler.GetTypes()
                        .Where(t => t.Name.EndsWith("Controller") && t.IsClass)
                        .ToArray();

                if (controllerTypes.Length == 0)
                    throw new Exception("Unable to generate service controllers");

                Service = new ServiceItem(controllerTypes.First()) {
                    Name = !string.IsNullOrWhiteSpace(Name) ? Name : "My Service"
                };
                Global.CurrentService = Service;

                foreach (var controller in controllerTypes.Skip(1)) {
                    var temp = new ServiceItem(controller);
                    Service.Methods.AddRange(temp.Methods);
                }

                AddServiceLogMessage = "Starting server...";
                var settings = new Settings { Enabled = true, PortNumber = PortNumber };
                var svc = new ServerService();
                svc.RestartServer(settings, controllerTypes);
                Service.Settings = settings;
                HasService = true;
                Global.Trace.Enabled = true;
                AddServiceLogMessage = $"Server is up and running at port {settings.PortNumber}";
            }
        }
        catch (Exception exc) {
            AddServiceLogMessage = $"Failed. {exc.Message}";
            //File.WriteAllText(file + ".compile-error.txt", exc.ToString());
            Debugger.Break();
        }
    }
}