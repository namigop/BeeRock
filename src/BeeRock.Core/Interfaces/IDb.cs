using System.Linq.Expressions;

namespace BeeRock.Core.Interfaces;

public interface IDb<T, S> : IDisposable
    where T : IDao
    where S : IDto {
    void Delete(string id);

    bool Exists(string id);

    List<T> Find(Expression<Func<S, bool>> predicate);

    T FindById(string id);

    T ToDao(S source);

    S ToDto(T source);

    void Upsert(string id, T dao);
}