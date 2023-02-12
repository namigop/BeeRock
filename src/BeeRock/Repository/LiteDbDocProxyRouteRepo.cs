using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public class LiteDbDocProxyRouteRepo : IDb<DocProxyRouteDao, DocProxyRouteDto> {
    private readonly LiteDatabase _db;

    public LiteDbDocProxyRouteRepo(LiteDatabase db) {
        _db = db;
    }

    public void Delete(string id) {
        var c = _db.GetCollection<DocProxyRouteDao>();
        c.EnsureIndex(t => t.DocId);
        c.Delete(id);
    }

    public void Dispose() {
        _db?.Dispose();
    }

    public bool Exists(string id) {
        var c = _db.GetCollection<DocProxyRouteDao>();
        c.EnsureIndex(t => t.DocId);
        return c.Exists(t => t.DocId == id);
    }

    public List<DocProxyRouteDao> Find(Expression<Func<DocProxyRouteDto, bool>> predicate) {
        var c = _db.GetCollection<DocProxyRouteDao>();
        c.EnsureIndex(t => t.DocId);
        Expression<Func<DocProxyRouteDao, bool>> filter = dao => predicate.Compile().Invoke(ToDto(dao));
        //return c.Find(filter).ToList();
        return c.FindAll().Where(filter.Compile()).ToList();
    }

    public DocProxyRouteDao FindById(string id) {
        var c = _db.GetCollection<DocProxyRouteDao>();
        c.EnsureIndex(t => t.DocId);
        return c.FindById(id);
    }

    public DocProxyRouteDao ToDao(DocProxyRouteDto source) {
        if (source is null)
            return null;

        return new DocProxyRouteDao {
            Index = source.Index,
            DocId = source.DocId,
            IsEnabled = source.IsEnabled,
            LastUpdated = source.LastUpdated,
            From = new ProxyRoutePartDao {
                Host = source.From.Host,
                Scheme = source.From.Scheme,
                PathTemplate = source.From.PathTemplate
            },
            To = new ProxyRoutePartDao {
                Host = source.To.Host,
                Scheme = source.To.Scheme,
                PathTemplate = source.To.PathTemplate
            }
        };
    }

    public DocProxyRouteDto ToDto(DocProxyRouteDao source) {
        if (source is null)
            return null;

        return new DocProxyRouteDto {
            Index = source.Index,
            DocId = source.DocId,
            IsEnabled = source.IsEnabled,
            LastUpdated = source.LastUpdated,
            From = new ProxyRoutePartDto {
                Host = source.From.Host,
                Scheme = source.From.Scheme,
                PathTemplate = source.From.PathTemplate
            },
            To = new ProxyRoutePartDto {
                Host = source.To.Host,
                Scheme = source.To.Scheme,
                PathTemplate = source.To.PathTemplate
            }
        };
    }

    public void Upsert(string id, DocProxyRouteDao entity) {
        var c = _db.GetCollection<DocProxyRouteDao>();
        c.EnsureIndex(t => t.DocId);
        c.Upsert(id, entity);
    }
}
