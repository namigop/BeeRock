using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public class LiteDbDocServiceRuleSetsRepo : IDb<DocServiceRuleSetsDao, DocServiceRuleSetsDto> {
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

    public List<DocServiceRuleSetsDao> Find(Expression<Func<DocServiceRuleSetsDto, bool>> predicate) {
        var c = _db.GetCollection<DocServiceRuleSetsDao>();
        c.EnsureIndex(t => t.DocId);
        Expression<Func<DocServiceRuleSetsDao, bool>> filter = dao => predicate.Compile().Invoke(ToDto(dao));
        //return c.Find(filter).ToList();

        return c.FindAll().Where(filter.Compile()).ToList();
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

    public DocServiceRuleSetsDto ToDto(DocServiceRuleSetsDao source) {
        var d = new DocServiceRuleSetsDto {
            DocId = source.DocId,
            LastUpdated = source.LastUpdated,
            PortNumber = source.PortNumber,
            ServiceName = source.ServiceName,
            SourceSwagger = source.SourceSwagger,
            Routes = source.Routes.Select(r => {
                var s = new RouteRuleSetsDto {
                    HttpMethod = r.HttpMethod,
                    MethodName = r.MethodName,
                    RouteTemplate = r.RouteTemplate,
                    RuleSetIds = r.RuleSetIds
                };
                return s;
            }).ToArray()
        };
        return d;
    }

    public DocServiceRuleSetsDao ToDao(DocServiceRuleSetsDto source) {
        var d = new DocServiceRuleSetsDao {
            DocId = source.DocId,
            LastUpdated = source.LastUpdated,
            PortNumber = source.PortNumber,
            ServiceName = source.ServiceName,
            SourceSwagger = source.SourceSwagger,
            Routes = source.Routes.Select(r => {
                var s = new RouteRuleSetsDao {
                    HttpMethod = r.HttpMethod,
                    MethodName = r.MethodName,
                    RouteTemplate = r.RouteTemplate,
                    RuleSetIds = r.RuleSetIds
                };
                return s;
            }).ToArray()
        };
        return d;
    }
}
