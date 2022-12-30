using BeeRock.Core.Entities;

namespace BeeRock.Core.Interfaces;

public interface IServerHostingService{
    Task StartServer();
    Task StopServer();
    bool CanStart { get; }
    bool CanStop { get; }
    string GetServerStatus();
}
