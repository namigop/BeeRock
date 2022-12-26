using System.Linq.Expressions;
using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;

namespace BeeRock.Adapters.Repository;

public class DocServiceRuleSetsRepo : IDocServiceRuleSetsRepo {
    private readonly IDb<DocServiceRuleSetsDao> _db;


    public DocServiceRuleSetsRepo(IDb<DocServiceRuleSetsDao> db) {
        _db = db;
    }


    public string Create(DocServiceRuleSetsDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.Routes, nameof(dao.Routes));
        Requires.NotNullOrEmpty(dao.ServiceName, nameof(dao.ServiceName));
        Requires.NotNullOrEmpty(dao.SourceSwagger, nameof(dao.SourceSwagger));
        Requires.IsTrue(() => dao.PortNumber > 100, nameof(dao.PortNumber));

        if (string.IsNullOrWhiteSpace(dao.DocId)) {
            dao.DocId = Guid.NewGuid().ToString();
        }

        lock (Db.DbLock) {
            _db.Upsert(dao.DocId, dao);
        }

        return dao.DocId;
    }

    public DocServiceRuleSetsDao Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));
        return _db.FindById(id);
    }

    public List<DocServiceRuleSetsDao> All() {
        return Where(x => true);
    }

    public void Update(DocServiceRuleSetsDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.DocId, nameof(dao.DocId));
        Requires.NotNullOrEmpty(dao.Routes, nameof(dao.Routes));
        Requires.NotNullOrEmpty(dao.ServiceName, nameof(dao.ServiceName));
        Requires.NotNullOrEmpty(dao.SourceSwagger, nameof(dao.SourceSwagger));
        Requires.IsTrue(() => dao.PortNumber > 100, nameof(dao.PortNumber));

        var d = _db.FindById(dao.DocId);
        d.SourceSwagger = dao.SourceSwagger;
        d.Routes = dao.Routes;
        d.ServiceName = dao.ServiceName;
        d.PortNumber = dao.PortNumber;
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

    public List<DocServiceRuleSetsDao> Where(Expression<Func<DocServiceRuleSetsDao, bool>> predicate) {
        return _db.Find(predicate);
    }

    public bool Exists(string id) {
        if (string.IsNullOrWhiteSpace(id)) {
            return false;
        }

        return _db.Exists(id);
    }
}
