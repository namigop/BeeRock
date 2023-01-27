using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public static class Db {
    public static object DbLock { get; } = new();
    private static LiteDatabase DbInstance { get; } = new(Global.DbFile);

    public static void Dispose() {
        DbInstance.Dispose();
    }

    public static IDb<DocRuleDao, DocRuleDto> GetRuleDb() {
        return new LiteDbDocRuleRepo(DbInstance);
    }

    public static IDb<DocServiceRuleSetsDao, DocServiceRuleSetsDto> GetServiceDb() {
        return new LiteDbDocServiceRuleSetsRepo(DbInstance);
    }
}