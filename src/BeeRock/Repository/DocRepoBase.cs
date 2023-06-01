using System.Linq.Expressions;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public abstract class DocRepoBase<Dao,Dto> : IRepository<Dto>
    where Dao:IDoc, IDao
    where Dto:IDoc, IDto {
    protected readonly IDb<Dao, Dto> _db;

    protected DocRepoBase(IDb<Dao, Dto> db) {
        _db = db;
    }
    public List<Dto> All() {
        return Where(x => true);
    }

    public void Shrink() {
        _db.Shrink();
    }

    public int Count() {
        return _db.Count();
    }

    public virtual string Create(Dto dto) {
        lock (Db.DbLock) {
            if (string.IsNullOrWhiteSpace(dto.DocId))
                dto.DocId = Guid.NewGuid().ToString();

            _db.Upsert(dto.DocId, _db.ToDao(dto));
        }

        return dto.DocId;
    }

    public void Delete(string docId) {
        Requires.NotNullOrEmpty(docId, nameof(docId));
        lock (Db.DbLock) {
            _db.Delete(docId);
        }
    }

    public void DeleteAll() {
        _db.DeleteAll();
    }

    public bool Exists(string id) {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _db.Exists(id);
    }

    public Dto Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));
        return _db.FindById(id).Then(t => _db.ToDto(t));
    }

    public abstract void Update(Dto dto);
    public List<Dto> Where(Expression<Func<Dto, bool>> predicate) {
        return _db.Find(predicate).Select(p => _db.ToDto(p)).ToList();
    }
}
