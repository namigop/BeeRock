using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public class DocRuleRepo : IDocRuleRepo {
    private readonly IDb<DocRuleDao, DocRuleDto> _db;

    public DocRuleRepo(IDb<DocRuleDao, DocRuleDto> db) {
        _db = db;
    }

    public List<DocRuleDto> All() {
        return Where(x => true);
    }

    public string Create(DocRuleDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNullOrEmpty(dto.Conditions, nameof(dto.Conditions));
        Requires.NotNullOrEmpty(dto.Name, nameof(dto.Name));
        Requires.IsTrue(() => dto.StatusCode > 100, nameof(dto.StatusCode));

        lock (Db.DbLock) {
            if (string.IsNullOrWhiteSpace(dto.DocId))
                dto.DocId = Guid.NewGuid().ToString();

            _db.Upsert(dto.DocId, _db.ToDao(dto));
        }

        return dto.DocId;
    }

    public void Delete(string docId) {
        Requires.NotNullOrEmpty(docId, nameof(docId));
        lock (Db.DbLock) {
            _db.Delete(docId);
        }
    }

    public bool Exists(string id) {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _db.Exists(id);
    }

    public DocRuleDto Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));
        return _db.FindById(id).Then(t => _db.ToDto(t));
    }

    public void Update(DocRuleDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNullOrEmpty(dto.DocId, nameof(dto.DocId));
        Requires.NotNullOrEmpty(dto.Conditions, nameof(dto.Conditions));
        Requires.NotNullOrEmpty(dto.Name, nameof(dto.Name));
        Requires.IsTrue(() => dto.StatusCode > 100, nameof(dto.StatusCode));

        var d = _db.FindById(dto.DocId);
        d.Conditions = dto.Conditions.Select(t => new WhenDao {
            BooleanExpression = t.BooleanExpression,
            IsActive = t.IsActive
        }).ToArray();
        d.StatusCode = dto.StatusCode;
        d.Name = dto.Name;
        d.Body = dto.Body;
        d.IsSelected = dto.IsSelected;
        lock (Db.DbLock) {
            _db.Upsert(d.DocId, d);
        }
    }

    public List<DocRuleDto> Where(Expression<Func<DocRuleDto, bool>> predicate) {
        return _db.Find(predicate).Select(p => _db.ToDto(p)).ToList();
    }
}
