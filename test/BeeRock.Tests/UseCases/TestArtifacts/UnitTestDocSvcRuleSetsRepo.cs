using System.Linq.Expressions;
using BeeRock.Ports.Repository;

namespace BeeRock.Tests.UseCases.TestArtifacts;

public class UnitTestDocSvcRuleSetsRepo : IDocServiceRuleSetsRepo {
    private readonly Dictionary<string, DocServiceRuleSetsDao> svcDb;

    public UnitTestDocSvcRuleSetsRepo(UnitTestDb db) {
        this.svcDb = db.svcDb;
    }

    public string Create(DocServiceRuleSetsDao dao) {
        if (string.IsNullOrWhiteSpace(dao.DocId))
            dao.DocId = Guid.NewGuid().ToString();

        svcDb[dao.DocId] = dao;
        return dao.DocId;
    }

    public DocServiceRuleSetsDao Read(string id) {
        return svcDb[id];
    }

    public List<DocServiceRuleSetsDao> Where(Expression<Func<DocServiceRuleSetsDao, bool>> predicate) {
        return svcDb.Values.Where(predicate.Compile()).ToList();
    }

    public List<DocServiceRuleSetsDao> All() {
        return svcDb.Values.ToList();
    }

    public void Update(DocServiceRuleSetsDao dao) {
        if (!svcDb.Keys.Contains(dao.DocId))
            throw new Exception("DocId not found");

        svcDb[dao.DocId] = dao;
    }

    public void Delete(string id) {
        if (!svcDb.Keys.Contains(id))
            throw new Exception("DocId not found");

        DocServiceRuleSetsDao dao;
        svcDb.Remove(id, out dao);
    }

    public bool Exists(string id) {
        return svcDb.Keys.Contains(id);
    }
}
