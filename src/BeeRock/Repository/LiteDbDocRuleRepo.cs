using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public class LiteDbDocRuleRepo : LiteDbBase<DocRuleDao, DocRuleDto> {
    public LiteDbDocRuleRepo(LiteDatabase db) : base(db) {
    }

    public override DocRuleDao ToDao(DocRuleDto source) {
        if (source is null)
            return null;

        return new DocRuleDao {
            Body = source.Body,
            Conditions = source.Conditions.Select(c => new WhenDao { BooleanExpression = c.BooleanExpression, IsActive = c.IsActive }).ToArray(),
            DelayMsec = source.DelayMsec,
            DocId = source.DocId,
            IsSelected = source.IsSelected,
            LastUpdated = source.LastUpdated,
            Name = source.Name,
            StatusCode = source.StatusCode
        };
    }

    public override DocRuleDto ToDto(DocRuleDao source) {
        if (source is null)
            return null;

        return new DocRuleDto {
            Body = source.Body,
            Conditions = source.Conditions.Select(c => new WhenDto { BooleanExpression = c.BooleanExpression, IsActive = c.IsActive }).ToArray(),
            DelayMsec = source.DelayMsec,
            DocId = source.DocId,
            IsSelected = source.IsSelected,
            LastUpdated = source.LastUpdated,
            Name = source.Name,
            StatusCode = source.StatusCode
        };
    }
}
