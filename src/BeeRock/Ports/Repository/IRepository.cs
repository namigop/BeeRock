using System.Linq.Expressions;

namespace BeeRock.Ports.Repository;

public interface IRepository<T> {
    Task<string> Create(T dao);
    Task<T> Read(string id);
    Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate);

    Task Update(T dao);
    Task Delete(T dao);

    Task<bool> Exists(string id);
}
