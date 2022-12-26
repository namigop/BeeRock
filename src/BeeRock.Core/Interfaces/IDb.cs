using System.Linq.Expressions;

namespace BeeRock.Core.Interfaces;

public interface IDb<T, S> : IDisposable
    where T : IDao
    where S:IDto {
    void Upsert(string id, T dao);
    List<T> Find(Expression<Func<S, bool>> predicate);

    T FindById(string id);
    void Delete(string id);
    bool Exists(string id);

    S ToDto(T source);
    T ToDao(S source);
}
