using System.Linq.Expressions;

namespace BeeRock.Ports.Repository;

public interface IRepository<T> {
    Task<string> Create(T dao);
    Task<T> Read(string id);
    Task<List<T>> Where(Expression<Func<T, bool>> predicate);
    Task<List<T>> All();

    Task Update(T dao);
    Task Delete(string id);


    Task<bool> Exists(string id);
}
