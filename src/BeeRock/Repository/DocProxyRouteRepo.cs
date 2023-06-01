using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public class DocProxyRouteRepo : DocRepoBase<DocProxyRouteDao, DocProxyRouteDto>, IDocProxyRouteRepo {

    public DocProxyRouteRepo(IDb<DocProxyRouteDao, DocProxyRouteDto> db) :base (db){
    }


    public override string Create(DocProxyRouteDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNull(dto.From, nameof(dto.From));
        Requires.NotNull(dto.To, nameof(dto.To));

        return base.Create(dto);
    }

    public override void Update(DocProxyRouteDto dao) {
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
}
