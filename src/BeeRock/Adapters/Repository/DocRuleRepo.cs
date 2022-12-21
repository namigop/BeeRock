using System.Linq.Expressions;
using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;

namespace BeeRock.Adapters.Repository;

public class DocRuleRepo : IDocRuleRepo {
    private readonly IDb<DocRuleDao> _db;


    public DocRuleRepo(IDb<DocRuleDao> db) {
        _db = db;
    }

    public string Create(DocRuleDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.Conditions, nameof(dao.Conditions));
        Requires.NotNullOrEmpty(dao.Name, nameof(dao.Name));
        Requires.IsTrue(() => dao.StatusCode > 100, nameof(dao.StatusCode));

        if (string.IsNullOrWhiteSpace(dao.DocId))
            dao.DocId = Guid.NewGuid().ToString();

        lock (Db.DbLock) {
            _db.Upsert(dao.DocId, dao);
        }

        return dao.DocId;
    }

    public DocRuleDao Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));

        return _db.FindById(id);
    }

    public List<DocRuleDao> Where(Expression<Func<DocRuleDao, bool>> predicate) {
        return _db.Find(predicate);
    }

    public List<DocRuleDao> All() {
        return Where(x => true);
    }

    public void Update(DocRuleDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.DocId, nameof(dao.DocId));
        Requires.NotNullOrEmpty(dao.Conditions, nameof(dao.Conditions));
        Requires.NotNullOrEmpty(dao.Name, nameof(dao.Name));
        Requires.IsTrue(() => dao.StatusCode > 100, nameof(dao.StatusCode));

        var d = _db.FindById(dao.DocId);
        d.Conditions = dao.Conditions;
        d.StatusCode = dao.StatusCode;
        d.Name = dao.Name;
        d.Body = dao.Body;
        d.IsSelected = dao.IsSelected;
        lock (Db.DbLock) {
            _db.Upsert(d.DocId, d);
        }
    }

    public void Delete(string docId) {
        Requires.NotNullOrEmpty(docId, nameof(docId));
        lock (Db.DbLock) {
            _db.Delete(docId);
        }
    }

    public bool Exists(string id) {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _db.Exists(id);
    }
}
