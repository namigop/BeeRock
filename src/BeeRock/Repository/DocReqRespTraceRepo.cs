using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public class DocReqRespTraceRepo : DocRepoBase<DocReqRespTraceDao, DocReqRespTraceDto>, IDocReqRespTraceRepo {

    public DocReqRespTraceRepo(IDb<DocReqRespTraceDao, DocReqRespTraceDto> db) :base(db) {
    }

    public override string Create(DocReqRespTraceDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNullOrEmpty(dto.StatusCode, nameof(dto.StatusCode));
        Requires.NotNullOrEmpty(dto.RequestUri, nameof(dto.RequestUri));
        return base.Create(dto);
    }



    public override void Update(DocReqRespTraceDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNullOrEmpty(dto.DocId, nameof(dto.DocId));

        var d = _db.FindById(dto.DocId);
        d.StatusCode = dto.StatusCode;
        d.RequestUri = dto.RequestUri;
        d.Timestamp= dto.Timestamp;
        d.ElapsedMsec = dto.ElapsedMsec;
        d.RequestBody = dto.RequestBody;
        d.RequestHeaders = dto.RequestHeaders;
        d.ResponseHeaders = dto.ResponseHeaders;
        d.RequestMethod = dto.RequestMethod;

        lock (Db.DbLock) {
            _db.Upsert(d.DocId, d);
        }
    }
}
