namespace BeeRock.Core.Interfaces;

public interface IServerHostingService {
    bool CanStart { get; }
    bool CanStop { get; }

    string GetServerStatus();

    Task StartServer();

    Task StopServer();
}