using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;
using LiteDB;


namespace BeeRock.Adapters.Repository;

public class DocRuleRepo : IDocRuleRepo {
    private readonly string _dbFilePath;

    public DocRuleRepo(string dbFilePath) {
        _dbFilePath = dbFilePath;
    }

    public Task<string> Create(DocRuleDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.Conditions, nameof(dao.Conditions));
        Requires.NotNullOrEmpty(dao.Name, nameof(dao.Name));
        Requires.IsTrue(() => dao.StatusCode > 100, nameof(dao.StatusCode));

        return Task.Run(() => {
            using var db = new LiteDatabase(_dbFilePath);
            if (string.IsNullOrWhiteSpace(dao.DocId))
                dao.DocId = Guid.NewGuid().ToString();

            var collection = db.GetCollection<DocRuleDao>();
            collection.EnsureIndex(d => d.DocId);
            collection.Insert(dao);
            return dao.DocId;
        });
    }

    public Task<DocRuleDao> Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));

        return Task.Run(() => {
            using var db = new LiteDatabase(_dbFilePath);

            var collection = db.GetCollection<DocRuleDao>();
            collection.EnsureIndex(d => d.DocId);
            var dao = collection.Query().Where(t => t.DocId == id).FirstOrDefault();
            return dao;
        });
    }

    public Task Update(DocRuleDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.DocId, nameof(dao.DocId));
        Requires.NotNullOrEmpty(dao.Conditions, nameof(dao.Conditions));
        Requires.NotNullOrEmpty(dao.Name, nameof(dao.Name));
        Requires.IsTrue(() => dao.StatusCode > 100, nameof(dao.StatusCode));

        return Task.Run(() => {
            using var db = new LiteDatabase(_dbFilePath);

            var collection = db.GetCollection<DocRuleDao>();
            collection.EnsureIndex(d => d.DocId);

            var d = collection.Query().Where(t => t.DocId == dao.DocId).FirstOrDefault();
            d.Conditions = dao.Conditions;
            d.StatusCode = dao.StatusCode;
            d.Name = dao.Name;
            d.Body = dao.Body;
            d.IsSelected = dao.IsSelected;

            collection.Update(d);
        });
    }

    public Task Delete(DocRuleDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.DocId, nameof(dao.DocId));
        return Task.Run(() => {
            using var db = new LiteDatabase(_dbFilePath);

            var collection = db.GetCollection<DocRuleDao>();
            collection.EnsureIndex(d => d.DocId);
            var d = collection.Delete(dao.DocId);

        });
    }
}
