namespace BeeRock.Ports.Repository;

public interface IRepository<T> {
    Task<string> Create(T dto);
    Task<T> Read(string id);
    Task Update(T dto);
    Task Delete(T dto);
}
