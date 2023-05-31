using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public class DocProxyRouteRepo : IDocProxyRouteRepo {
    private readonly IDb<DocProxyRouteDao, DocProxyRouteDto> _db;

    public DocProxyRouteRepo(IDb<DocProxyRouteDao, DocProxyRouteDto> db) {
        _db = db;
    }

    public List<DocProxyRouteDto> All() {
        return Where(x => true);
    }

    public string Create(DocProxyRouteDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNull(dto.From, nameof(dto.From));
        Requires.NotNull(dto.To, nameof(dto.To));

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

    public void DeleteAll() {
        _db.DeleteAll();
    }

    public bool Exists(string id) {
        if (string.IsNullOrWhiteSpace(id)) return false;

        return _db.Exists(id);
    }

    public DocProxyRouteDto Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));
        return _db.FindById(id).Then(t => _db.ToDto(t));
    }

    public void Update(DocProxyRouteDto dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.DocId, nameof(dao.DocId));
        Requires.NotNull(dao.From, nameof(dao.From));
        Requires.NotNull(dao.To, nameof(dao.To));

        var d = _db.FindById(dao.DocId);
        d.Index = dao.Index;
        d.IsEnabled = dao.IsEnabled;
        d.From = new ProxyRoutePartDao {
            Host = dao.From.Host,
            Scheme = dao.From.Scheme,
            PathTemplate = dao.From.PathTemplate
        };
        d.To = new ProxyRoutePartDao {
            Host = dao.To.Host,
            Scheme = dao.To.Scheme,
            PathTemplate = dao.To.PathTemplate
        };

        lock (Db.DbLock) {
            _db.Upsert(d.DocId, d);
        }
    }

    public List<DocProxyRouteDto> Where(Expression<Func<DocProxyRouteDto, bool>> predicate) {
        return _db.Find(predicate).Select(c => _db.ToDto(c)).ToList();
    }
}
