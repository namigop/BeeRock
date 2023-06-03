using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;

namespace BeeRock.Tests.UseCases.Fakes;

public class FakeDocRuleRepo : IDocRuleRepo {
    private readonly Dictionary<string, DocRuleDto> ruleDb;

    public FakeDocRuleRepo(FakeDb db) {
        ruleDb = db.ruleDb;
    }

    public int Count() {
        return ruleDb.Values.Count;
    }

    public string Create(DocRuleDto dto) {
        if (string.IsNullOrWhiteSpace(dto.DocId))
            dto.DocId = Guid.NewGuid().ToString();

        ruleDb[dto.DocId] = dto;
        return dto.DocId;
    }

    public DocRuleDto Read(string id) {
        return ruleDb[id];
    }

    public List<DocRuleDto> Where(Expression<Func<DocRuleDto, bool>> predicate) {
        return ruleDb.Values.Where(predicate.Compile()).ToList();
    }

    public List<DocRuleDto> All() {
        return ruleDb.Values.ToList();
    }

    public void Shrink() {
        //do nothing
    }

    public void Update(DocRuleDto dto) {
        if (!ruleDb.Keys.Contains(dto.DocId))
            throw new Exception("DocId not found");

        ruleDb[dto.DocId] = dto;
    }

    public void Delete(string id) {
        if (!ruleDb.Keys.Contains(id))
            throw new Exception("DocId not found");

        DocRuleDto dto;
        ruleDb.Remove(id, out dto);
    }

    public void DeleteAll() {
        ruleDb.Clear();
    }

    public bool Exists(string id) {
        return ruleDb.Keys.Contains(id);
    }
}
