using System.Linq.Expressions;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public abstract class LiteDbBase<Dao, Dto> : IDb<Dao, Dto>
    where Dao : IDao , IDoc
    where Dto : IDto {
    private readonly LiteDatabase _db;

    protected LiteDbBase(LiteDatabase db) {
        _db = db;
    }

    public void Dispose() {
        _db?.Dispose();
    }

    public void Delete(string id) {
        var c = _db.GetCollection<Dao>();
        c.EnsureIndex(t => t.DocId);
        c.Delete(id);
    }

    public void DeleteAll() {
        var c = _db.GetCollection<Dao>();
        c.DeleteAll();
    }


    public bool Exists(string id) {
        var c = _db.GetCollection<Dao>();
        c.EnsureIndex(t => t.DocId);

        return c.Exists(t => t.DocId == id);
    }

    public List<Dao> Find(Expression<Func<Dto, bool>> predicate) {
        var c = _db.GetCollection<Dao>();
        c.EnsureIndex(t => t.DocId);
        Expression<Func<Dao, bool>> filter = dao => predicate.Compile().Invoke(ToDto(dao));
        //return c.Find(filter).ToList();
        var d = c.FindAll();
        return
            d
                .Where(filter.Compile())
                .ToList();
    }

    public Dao FindById(string id) {
        var c = _db.GetCollection<Dao>();
        c.EnsureIndex(t => t.DocId);
        return c.FindById(id);
    }

    public abstract Dao ToDao(Dto source);

    public abstract Dto ToDto(Dao source);
    public void Upsert(string id, Dao entity) {
        var c = _db.GetCollection<Dao>();
        c.EnsureIndex(t => t.DocId);
        c.Upsert(id, entity);
    }
}
