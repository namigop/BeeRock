namespace BeeRock.Core.Interfaces;

public interface IServerHostingService {
    bool CanStart { get; }
    bool CanStop { get; }
    Task StartServer();
    Task StopServer();
    string GetServerStatus();
}
