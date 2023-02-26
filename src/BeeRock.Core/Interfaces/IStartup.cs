using Microsoft.Extensions.Configuration;

namespace BeeRock.Core.Interfaces;

public interface IStartup {
    string ServiceName { get; }
    IStartup Setup(IConfiguration configuration);
}
