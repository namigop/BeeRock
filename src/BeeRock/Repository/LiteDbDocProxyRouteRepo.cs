using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public class LiteDbDocProxyRouteRepo : LiteDbBase<DocProxyRouteDao, DocProxyRouteDto> {
    public LiteDbDocProxyRouteRepo(LiteDatabase db) : base(db) {
    }

    public override DocProxyRouteDao ToDao(DocProxyRouteDto source) {
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

    public override DocProxyRouteDto ToDto(DocProxyRouteDao source) {
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
}
