using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;
using LiteDB;

namespace BeeRock.Adapters.Repository;

public class DocServiceRuleSetsRepo :  IDocServiceRuleSetsRepo {
    private readonly string _dbFilePath;

    public DocServiceRuleSetsRepo(string dbFilePath) {
        _dbFilePath = dbFilePath;
    }

    public Task<string> Create(DocServiceRuleSetsDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.Routes, nameof(dao.Routes));
        Requires.NotNullOrEmpty(dao.ServiceName, nameof(dao.ServiceName));
        Requires.NotNullOrEmpty(dao.SourceSwagger, nameof(dao.SourceSwagger));
        Requires.IsTrue(() => dao.PortNumber > 100, nameof(dao.PortNumber));

        return Task.Run(() => {
            using var db = new LiteDatabase(_dbFilePath);
            if (string.IsNullOrWhiteSpace(dao.DocId))
                dao.DocId = Guid.NewGuid().ToString();

            var collection = db.GetCollection<DocServiceRuleSetsDao>();
            collection.EnsureIndex(d => d.DocId);
            collection.Insert(dao);
            return dao.DocId;
        });
    }

    public Task<DocServiceRuleSetsDao> Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));

        return Task.Run(() => {
            using var db = new LiteDatabase(_dbFilePath);

            var collection = db.GetCollection<DocServiceRuleSetsDao>();
            collection.EnsureIndex(d => d.DocId);
            var dao = collection.Query().Where(t => t.DocId == id).FirstOrDefault();
            return dao;
        });
    }

    public Task Update(DocServiceRuleSetsDao dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.DocId, nameof(dao.DocId));
        Requires.NotNullOrEmpty(dao.Routes, nameof(dao.Routes));
        Requires.NotNullOrEmpty(dao.ServiceName, nameof(dao.ServiceName));
        Requires.NotNullOrEmpty(dao.SourceSwagger, nameof(dao.SourceSwagger));
        Requires.IsTrue(() => dao.PortNumber > 100, nameof(dao.PortNumber));

        return Task.Run(() => {
            using var db = new LiteDatabase(_dbFilePath);

            var collection = db.GetCollection<DocServiceRuleSetsDao>();
            collection.EnsureIndex(d => d.DocId);

            var d = collection.Query().Where(t => t.DocId == dao.DocId).FirstOrDefault();
            d.SourceSwagger = dao.SourceSwagger;
            d.Routes = dao.Routes;
            d.ServiceName = dao.ServiceName;
            d.PortNumber = dao.PortNumber;

            collection.Update(d);
        });
    }

    public Task Delete(DocServiceRuleSetsDao dao) {
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
