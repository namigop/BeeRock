using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Ports.Repository;
using LiteDB;

namespace BeeRock.Adapters.Repository;

public static class Db {
    private static LiteDatabase DbInstance { get; } = new(Global.DbFile);
    public static object DbLock { get; } = new();

    public static IDb<DocRuleDao, DocRuleDto> GetRuleDb() {
        return new LiteDbDocRuleRepo(DbInstance);
    }

    public static IDb<DocServiceRuleSetsDao, DocServiceRuleSetsDto> GetServiceDb() {
        return new LiteDbDocServiceRuleSetsRepo(DbInstance);
    }

    public static void Dispose() {
        DbInstance.Dispose();
    }
}
