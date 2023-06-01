using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public class DocRuleRepo : DocRepoBase<DocRuleDao, DocRuleDto>, IDocRuleRepo {

    public DocRuleRepo(IDb<DocRuleDao, DocRuleDto> db) :base(db) {
    }

    public override string Create(DocRuleDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNullOrEmpty(dto.Conditions, nameof(dto.Conditions));
        Requires.NotNullOrEmpty(dto.Name, nameof(dto.Name));
        Requires.IsTrue(() => dto.StatusCode > 100, nameof(dto.StatusCode));

        return base.Create(dto);

    }

    public override void Update(DocRuleDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNullOrEmpty(dto.DocId, nameof(dto.DocId));
        Requires.NotNullOrEmpty(dto.Conditions, nameof(dto.Conditions));
        Requires.NotNullOrEmpty(dto.Name, nameof(dto.Name));
        Requires.IsTrue(() => dto.StatusCode > 100, nameof(dto.StatusCode));

        var d = _db.FindById(dto.DocId);
        d.Conditions = dto.Conditions
            .Select(t => new WhenDao {
                    BooleanExpression = t.BooleanExpression,
                    IsActive = t.IsActive})
            .ToArray();

        d.StatusCode = dto.StatusCode;
        d.Name = dto.Name;
        d.Body = dto.Body;
        d.IsSelected = dto.IsSelected;
        lock (Db.DbLock) {
            _db.Upsert(d.DocId, d);
        }
    }
}
