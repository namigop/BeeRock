using Microsoft.Extensions.Configuration;

namespace BeeRock.Core.Entities;

public interface IStartup {
    string ServiceName { get; }
    IStartup Setup(IConfiguration configuration);
}
