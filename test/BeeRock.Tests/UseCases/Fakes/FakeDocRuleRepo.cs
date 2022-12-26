using System.Linq.Expressions;
using BeeRock.Ports.Repository;

namespace BeeRock.Tests.UseCases.Fakes;

public class FakeDocRuleRepo : IDocRuleRepo {
    private readonly Dictionary<string, DocRuleDao> ruleDb;

    public FakeDocRuleRepo(FakeDb db) {
        this.ruleDb = db.ruleDb;
    }

    public string Create(DocRuleDao dao) {
        if (string.IsNullOrWhiteSpace(dao.DocId))
            dao.DocId = Guid.NewGuid().ToString();

        ruleDb[dao.DocId] = dao;
        return dao.DocId;
    }

    public DocRuleDao Read(string id) {
        return ruleDb[id];
    }

    public List<DocRuleDao> Where(Expression<Func<DocRuleDao, bool>> predicate) {
        return ruleDb.Values.Where(predicate.Compile()).ToList();
    }

    public List<DocRuleDao> All() {
        return ruleDb.Values.ToList();
    }

    public void Update(DocRuleDao dao) {
        if (!ruleDb.Keys.Contains(dao.DocId))
            throw new Exception("DocId not found");

        ruleDb[dao.DocId] = dao;
    }

    public void Delete(string id) {
        if (!ruleDb.Keys.Contains(id))
            throw new Exception("DocId not found");

        DocRuleDao dao;
        ruleDb.Remove(id, out dao);
    }

    public bool Exists(string id) {
        return ruleDb.Keys.Contains(id);
    }
}
