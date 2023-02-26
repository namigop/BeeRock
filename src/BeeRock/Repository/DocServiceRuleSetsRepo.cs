using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public class DocServiceRuleSetsRepo : IDocServiceRuleSetsRepo {
    private readonly IDb<DocServiceRuleSetsDao, DocServiceRuleSetsDto> _db;

    public DocServiceRuleSetsRepo(IDb<DocServiceRuleSetsDao, DocServiceRuleSetsDto> db) {
        _db = db;
    }

    public List<DocServiceRuleSetsDto> All() {
        return Where(x => true);
    }

    public string Create(DocServiceRuleSetsDto dto) {
        Requires.NotNull(dto, nameof(dto));
        //Requires.NotNullOrEmpty(dto.Routes, nameof(dto.Routes));
        Requires.NotNullOrEmpty(dto.ServiceName, nameof(dto.ServiceName));
        //Requires.NotNullOrEmpty(dto.SourceSwagger, nameof(dto.SourceSwagger));
        Requires.IsTrue(() => dto.PortNumber > 100, nameof(dto.PortNumber));

        if (string.IsNullOrWhiteSpace(dto.DocId)) dto.DocId = Guid.NewGuid().ToString();

        lock (Db.DbLock) {
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

    public bool Exists(string id) {
        if (string.IsNullOrWhiteSpace(id)) return false;

        return _db.Exists(id);
    }

    public DocServiceRuleSetsDto Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));
        return _db.FindById(id).Then(t => _db.ToDto(t));
    }

    public void Update(DocServiceRuleSetsDto dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.DocId, nameof(dao.DocId));
        Requires.NotNullOrEmpty(dao.ServiceName, nameof(dao.ServiceName));
        Requires.IsTrue(() => dao.PortNumber > 100, nameof(dao.PortNumber));

        if (!dao.IsDynamic) {
            Requires.NotNullOrEmpty(dao.SourceSwagger, nameof(dao.SourceSwagger));
            Requires.NotNullOrEmpty(dao.Routes, nameof(dao.Routes));
        }

        var d = _db.FindById(dao.DocId);
        d.SourceSwagger = dao.SourceSwagger;
        d.Routes = dao.Routes.Select(t => new RouteRuleSetsDao {
            HttpMethod = t.HttpMethod,
            MethodName = t.MethodName,
            RouteTemplate = t.RouteTemplate,
            RuleSetIds = t.RuleSetIds
        }).ToArray();
        d.ServiceName = dao.ServiceName;
        d.PortNumber = dao.PortNumber;
        lock (Db.DbLock) {
            _db.Upsert(d.DocId, d);
        }
    }

    public List<DocServiceRuleSetsDto> Where(Expression<Func<DocServiceRuleSetsDto, bool>> predicate) {
        return _db.Find(predicate).Select(c => _db.ToDto(c)).ToList();
    }
}
