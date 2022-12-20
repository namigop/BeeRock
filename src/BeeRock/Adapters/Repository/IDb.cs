using System.Linq.Expressions;

namespace BeeRock.Adapters.Repository;

public interface IDb<T> : IDisposable where T : class {
    void Upsert(string id, T entity);
    List<T> Find(Expression<Func<T, bool>> predicate);

    T FindById(string id);
    void Delete(string id);
    bool Exists(string id);
}
