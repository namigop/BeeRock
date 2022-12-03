using System.Reflection;
using System.Runtime.Loader;

namespace BeeRock.Core.Entities;

public class SimpleAssemblyLoadContext : AssemblyLoadContext {
    protected override Assembly Load(AssemblyName assemblyName) {
        return null;
    }
}