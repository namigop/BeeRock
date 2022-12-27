using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;

namespace BeeRock.Tests.UseCases.Fakes;

public class FakeDocSvcRuleSetsRepo : IDocServiceRuleSetsRepo {
    private readonly Dictionary<string, DocServiceRuleSetsDto> svcDb;

    public FakeDocSvcRuleSetsRepo(FakeDb db) {
        svcDb = db.svcDb;
    }

    public string Create(DocServiceRuleSetsDto dto) {
        if (string.IsNullOrWhiteSpace(dto.DocId))
            dto.DocId = Guid.NewGuid().ToString();

        svcDb[dto.DocId] = dto;
        return dto.DocId;
    }

    public DocServiceRuleSetsDto Read(string id) {
        return svcDb[id];
    }

    public List<DocServiceRuleSetsDto> Where(Expression<Func<DocServiceRuleSetsDto, bool>> predicate) {
        return svcDb.Values.Where(predicate.Compile()).ToList();
    }

    public List<DocServiceRuleSetsDto> All() {
        return svcDb.Values.ToList();
    }

    public void Update(DocServiceRuleSetsDto dto) {
        if (!svcDb.Keys.Contains(dto.DocId))
            throw new Exception("DocId not found");

        svcDb[dto.DocId] = dto;
    }

    public void Delete(string id) {
        if (!svcDb.Keys.Contains(id))
            throw new Exception("DocId not found");

        DocServiceRuleSetsDto dto;
        svcDb.Remove(id, out dto);
    }

    public bool Exists(string id) {
        return svcDb.Keys.Contains(id);
    }
}
