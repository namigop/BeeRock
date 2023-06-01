using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public class DocServiceRuleSetsRepo : DocRepoBase<DocServiceRuleSetsDao, DocServiceRuleSetsDto>, IDocServiceRuleSetsRepo {
    public DocServiceRuleSetsRepo(IDb<DocServiceRuleSetsDao, DocServiceRuleSetsDto> db) : base(db) {
    }

    public override string Create(DocServiceRuleSetsDto dto) {
        Requires.NotNull(dto, nameof(dto));
        //Requires.NotNullOrEmpty(dto.Routes, nameof(dto.Routes));
        Requires.NotNullOrEmpty(dto.ServiceName, nameof(dto.ServiceName));
        //Requires.NotNullOrEmpty(dto.SourceSwagger, nameof(dto.SourceSwagger));
        Requires.IsTrue(() => dto.PortNumber > 100, nameof(dto.PortNumber));

        return base.Create(dto);
    }


    public override void Update(DocServiceRuleSetsDto dao) {
        Requires.NotNull(dao, nameof(dao));
        Requires.NotNullOrEmpty(dao.DocId, nameof(dao.DocId));
        Requires.NotNullOrEmpty(dao.ServiceName, nameof(dao.ServiceName));
        Requires.IsTrue(() => dao.PortNumber > 100, nameof(dao.PortNumber));

        if (!dao.IsDynamic) {
            Requires.NotNullOrEmpty(dao.SourceSwagger, nameof(dao.SourceSwagger));
            Requires.NotNullOrEmpty(dao.Routes, nameof(dao.Routes));
        }

        var d = _db.FindById(dao.DocId);
        d.SourceSwagger = dao.SourceSwagger;
        d.Routes = dao.Routes.Select(t => new RouteRuleSetsDao {
            HttpMethod = t.HttpMethod,
            MethodName = t.MethodName,
            RouteTemplate = t.RouteTemplate,
            RuleSetIds = t.RuleSetIds
        }).ToArray();
        d.ServiceName = dao.ServiceName;
        d.PortNumber = dao.PortNumber;
        lock (Db.DbLock) {
            _db.Upsert(d.DocId, d);
        }
    }
}
