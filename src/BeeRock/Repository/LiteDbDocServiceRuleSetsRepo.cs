using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public class LiteDbDocServiceRuleSetsRepo : LiteDbBase<DocServiceRuleSetsDao, DocServiceRuleSetsDto> {
    public LiteDbDocServiceRuleSetsRepo(LiteDatabase db) : base(db) {
    }

    public override DocServiceRuleSetsDao ToDao(DocServiceRuleSetsDto source) {
        if (source is null)
            return null;

        var d = new DocServiceRuleSetsDao {
            IsDynamic = source.IsDynamic,
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

    public override DocServiceRuleSetsDto ToDto(DocServiceRuleSetsDao source) {
        if (source is null)
            return null;

        var d = new DocServiceRuleSetsDto {
            IsDynamic = source.IsDynamic,
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
}
