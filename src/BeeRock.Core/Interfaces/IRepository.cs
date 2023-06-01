using System.Linq.Expressions;

namespace BeeRock.Core.Interfaces;

public interface IRepository<T> where T:IDoc {
    List<T> All();

    void Shrink();
    int Count();
    string Create(T dao);

    void Delete(string id);
    void DeleteAll();

    bool Exists(string id);

    T Read(string id);

    void Update(T dao);

    List<T> Where(Expression<Func<T, bool>> predicate);
}
