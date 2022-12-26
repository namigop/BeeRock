using System.Linq.Expressions;

namespace BeeRock.Core.Interfaces;

public interface IRepository<T> {
    string Create(T dao);
    T Read(string id);
    List<T> Where(Expression<Func<T, bool>> predicate);
    List<T> All();

    void Update(T dao);
    void Delete(string id);

    bool Exists(string id);
}
