using BeeRock.Core.Entities;
using BeeRock.ViewModels;
using Newtonsoft.Json;
using ReactiveUI;

namespace BeeRock.Models;

public class ServiceMethodItem : ViewModelBase {
    private RestMethodInfo _method;
    private string _responseText;
    private bool canShow = true;

    private static object NewSystemType(Type type, int counter) {
        object val = null;
        if (type == typeof(string))
            val = "CountryX";
        else if (type == typeof(int))
            val = 0;
        else if (type == typeof(double))
            val = 0.0;
        else if (type == typeof(bool))
            val = false;
        else if (type == typeof(float))
            val = 0.0f;
        else if (type == typeof(DateTime))
            val = DateTime.Now;
        else if (type.FullName.StartsWith("System.Nullable")) {
            var itemType = type.GenericTypeArguments.First();
            var itemInstance = NewSystemType(itemType, counter) ?? Populate(Activator.CreateInstance(itemType));
            val = itemInstance;
        }
        else if (type.FullName.StartsWith("System.Collections.Generic.List")) {
            var listInstance = Activator.CreateInstance(type);
            var itemType = type.GenericTypeArguments.First();
            var itemInstance = NewSystemType(itemType, counter) ??
                               Populate(Activator.CreateInstance(itemType), counter);
            var addM = type.GetMethod("Add");
            addM.Invoke(listInstance, new object[] { itemInstance });
            val = listInstance;
        }
        else if (type.FullName.StartsWith("System.Collections.Generic.Dictionary")) {
            var dictionary = Activator.CreateInstance(type);
            var keyType = type.GenericTypeArguments[0];
            var valType = type.GenericTypeArguments[1];


            var keyInstance = NewSystemType(keyType, counter) ??
                              Populate(Activator.CreateInstance(keyType), counter);
            var valueInstance = NewSystemType(valType, counter) ??
                                Populate(Activator.CreateInstance(valType), counter);

            var m = type.GetMethod("Add");
            m.Invoke(dictionary, new[] { keyInstance, valueInstance });
            val = dictionary;
        }

        else if (type.IsEnum) {
            var r = Enum.GetNames(type);
            val = Enum.Parse(type, r[0]);
        }

        return val;
    }

    public static object Populate(object instance, int counter = 0) {
        if (counter > 0)
            return null;
        counter += 1;
        if (instance == null)
            return null;

        var instanceType = instance.GetType();
        if (instanceType.FullName.StartsWith("System.")) {
            return NewSystemType(instanceType, counter);
        }

        try {
            var props = instanceType.GetProperties().Where(p => p.CanWrite);
            foreach (var prop in props) {
                var val = NewSystemType(prop.PropertyType, counter);
                if (val == null) {
                    val = Activator.CreateInstance(prop.PropertyType);
                    Populate(val, counter);
                }

                prop.SetValue(instance, val);
            }

            return instance;
        }
        catch {
            return null;
        }
        finally {
            counter += 1;
        }
    }

    public ServiceMethodItem(RestMethodInfo info) {
        Method = info;
        if (info.ReturnType != typeof(void)) {
            object instance;
            if (info.ReturnType.FullName.StartsWith("System."))
                instance = NewSystemType(info.ReturnType, 0);
            else
                instance = Populate(Activator.CreateInstance(info.ReturnType), 0);

            try {
                var json = JsonConvert.SerializeObject(instance, Formatting.Indented);
                ResponseText = json;
            }
            catch {
            }
        }
    }

    public string ResponseText {
        get => _responseText;
        set => this.RaiseAndSetIfChanged(ref _responseText, value);
    }

    public RestMethodInfo Method {
        get => _method;
        set => this.RaiseAndSetIfChanged(ref _method, value);
    }

    public string Color {
        get {
            if (Method.HttpMethod.ToUpper() == "POST")
                return HttpMethodColor.Post;
            if (Method.HttpMethod.ToUpper() == "PUT")
                return HttpMethodColor.Put;
            if (Method.HttpMethod.ToUpper() == "DELETE")
                return HttpMethodColor.Delete;

            return HttpMethodColor.Get;
        }
    }

    public string Color2 { get; } = "WhiteSmoke";


    public bool CanShow {
        get => canShow;
        set => this.RaiseAndSetIfChanged(ref canShow, value);
    }
}
