using System.Reflection;
using System.Runtime.Loader;

namespace Bym.Core.CS.Reflection;

public class SimpleAssemblyLoadContext : AssemblyLoadContext {
    protected override Assembly Load(AssemblyName assemblyName) {
        return null;
    }
}