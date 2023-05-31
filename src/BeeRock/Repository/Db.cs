using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public static class Db {
    public static object DbLock { get; } = new();
    private static LiteDatabase DbInstance { get; } = new(Global.DbFile);
    private static LiteDatabase TraceDbInstance { get; } = new(Global.DbTraceFile);

    public static void Dispose() {
        DbInstance.Dispose();
        TraceDbInstance.Dispose();
    }

    public static IDb<DocRuleDao, DocRuleDto> GetRuleDb() {
        return new LiteDbDocRuleRepo(DbInstance);
    }

    public static IDb<DocReqRespTraceDao, DocReqRespTraceDto> GetTraceDb() {
        return new LiteDbDocReqRespTraceRepo(TraceDbInstance);
    }
    public static IDb<DocServiceRuleSetsDao, DocServiceRuleSetsDto> GetServiceDb() {
        return new LiteDbDocServiceRuleSetsRepo(DbInstance);
    }

    public static IDb<DocProxyRouteDao, DocProxyRouteDto> GetProxyRouteDb() {
        return new LiteDbDocProxyRouteRepo(DbInstance);
    }
}
