using BeeRock.Ports.Repository;
using LiteDB;

namespace BeeRock.Adapters.Repository;

public static class Db {
    private static LiteDatabase DbInstance { get; } = new(Global.DbFile);

    public static IDb<DocRuleDao> GetRuleDb() {
        return new LiteDbDocRuleRepo(DbInstance);
    }

    public static IDb<DocServiceRuleSetsDao> GetServiceDb() {
        return new LiteDbDocServiceRuleSetsRepo(DbInstance);
    }

    public static void Dispose() {
        DbInstance.Dispose();
    }
}
