using System.Linq.Expressions;
using BeeRock.Ports.Repository;
using LiteDB;

namespace BeeRock.Adapters.Repository;

public class LiteDbDocRuleRepo : IDb<DocRuleDao> {
    private readonly LiteDatabase _db;

    public LiteDbDocRuleRepo(LiteDatabase db) {
        _db = db;
    }

    public void Dispose() {
        _db?.Dispose();
    }

    public void Upsert(string id, DocRuleDao entity) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        c.Upsert(id, entity);
    }

    public List<DocRuleDao> Find(Expression<Func<DocRuleDao, bool>> predicate) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        return c.Find(predicate).ToList();
    }


    public DocRuleDao FindById(string id) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        return c.FindById(id);
    }

    public void Delete(string id) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        c.Delete(id);
    }


    public bool Exists(string id) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        return c.Exists(t => t.DocId == id);
    }
}
