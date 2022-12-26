using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;
using LiteDB;

namespace BeeRock.Adapters.Repository;

public class LiteDbDocRuleRepo : IDb<DocRuleDao, DocRuleDto> {
    private readonly LiteDatabase _db;

    public LiteDbDocRuleRepo(LiteDatabase db) {
        _db = db;
    }

    public void Dispose() {
        _db?.Dispose();
    }

    public void Upsert(string id, DocRuleDao entity) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        c.Upsert(id, entity);
    }


    public List<DocRuleDao> Find(Expression<Func<DocRuleDto, bool>> predicate) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        Expression<Func<DocRuleDao, bool>> filter = dao => predicate.Compile().Invoke(ToDto(dao));
        //return c.Find(filter).ToList();
        return c.FindAll().Where(filter.Compile()).ToList();
    }


    public DocRuleDao FindById(string id) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        return c.FindById(id);
    }

    public void Delete(string id) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        c.Delete(id);
    }


    public bool Exists(string id) {
        var c = _db.GetCollection<DocRuleDao>();
        c.EnsureIndex(t => t.DocId);
        return c.Exists(t => t.DocId == id);
    }

    public DocRuleDto ToDto(DocRuleDao source) {
        return new DocRuleDto() {
            Body = source.Body,
            Conditions = source.Conditions.Select(c => new WhenDto() { BooleanExpression = c.BooleanExpression, IsActive = c.IsActive }).ToArray(),
            DelayMsec = source.DelayMsec,
            DocId = source.DocId,
            IsSelected = source.IsSelected,
            LastUpdated = source.LastUpdated,
            Name = source.Name,
            StatusCode = source.StatusCode
        };
    }

    public DocRuleDao ToDao(DocRuleDto source) {
        return new DocRuleDao() {
            Body = source.Body,
            Conditions = source.Conditions.Select(c => new WhenDao() { BooleanExpression = c.BooleanExpression, IsActive = c.IsActive }).ToArray(),
            DelayMsec = source.DelayMsec,
            DocId = source.DocId,
            IsSelected = source.IsSelected,
            LastUpdated = source.LastUpdated,
            Name = source.Name,
            StatusCode = source.StatusCode
        };
    }
}
