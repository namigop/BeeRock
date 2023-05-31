using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Repository;

public class DocReqRespTraceRepo : IDocReqRespTraceRepo {
    private readonly IDb<DocReqRespTraceDao, DocReqRespTraceDto> _db;

    public DocReqRespTraceRepo(IDb<DocReqRespTraceDao, DocReqRespTraceDto> db) {
        _db = db;
    }

    public List<DocReqRespTraceDto> All() {
        return Where(x => true);
    }

    public string Create(DocReqRespTraceDto dto) {
        Requires.NotNull(dto, nameof(dto));
        Requires.NotNullOrEmpty(dto.StatusCode, nameof(dto.StatusCode));
        Requires.NotNullOrEmpty(dto.RequestUri, nameof(dto.RequestUri));

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

    public void DeleteAll() {
       _db.DeleteAll();
    }

    public bool Exists(string id) {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _db.Exists(id);
    }

    public DocReqRespTraceDto Read(string id) {
        Requires.NotNullOrEmpty(id, nameof(id));
        return _db.FindById(id).Then(t => _db.ToDto(t));
    }

    public void Update(DocReqRespTraceDto dto) {
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

    public List<DocReqRespTraceDto> Where(Expression<Func<DocReqRespTraceDto, bool>> predicate) {
        return _db.Find(predicate).Select(p => _db.ToDto(p)).ToList();
    }
}
