namespace BeeRock.Core.Ports.SaveServiceDetailsUseCase;

public interface ISaveServiceDetailsUseCase {
    Task Save(string docId, string serviceName, int port, string swagger);
}
