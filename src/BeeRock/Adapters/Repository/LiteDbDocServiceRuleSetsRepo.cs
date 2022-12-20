using System.Linq.Expressions;
using BeeRock.Ports.Repository;
using LiteDB;

namespace BeeRock.Adapters.Repository;

public class LiteDbDocServiceRuleSetsRepo : IDb<DocServiceRuleSetsDao> {
    private readonly LiteDatabase _db;

    public LiteDbDocServiceRuleSetsRepo(LiteDatabase db) {
        _db = db;
    }

    public void Dispose() {
        _db?.Dispose();
    }

    public void Upsert(string id, DocServiceRuleSetsDao entity) {
        var c = _db.GetCollection<DocServiceRuleSetsDao>();
        c.EnsureIndex(t => t.DocId);
        c.Upsert(id, entity);
    }

    public List<DocServiceRuleSetsDao> Find(Expression<Func<DocServiceRuleSetsDao, bool>> predicate) {
        var c = _db.GetCollection<DocServiceRuleSetsDao>();
        c.EnsureIndex(t => t.DocId);
        return c.Find(predicate).ToList();
    }


    public DocServiceRuleSetsDao FindById(string id) {
        var c = _db.GetCollection<DocServiceRuleSetsDao>();
        c.EnsureIndex(t => t.DocId);
        return c.FindById(id);
    }

    public void Delete(string id) {
        var c = _db.GetCollection<DocServiceRuleSetsDao>();
        c.EnsureIndex(t => t.DocId);
        c.Delete(id);
    }


    public bool Exists(string id) {
        var c = _db.GetCollection<DocServiceRuleSetsDao>();
        c.EnsureIndex(t => t.DocId);
        return c.Exists(t => t.DocId == id);
    }
}
