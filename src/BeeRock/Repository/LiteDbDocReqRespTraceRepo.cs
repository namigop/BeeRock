using System.Linq.Expressions;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public class LiteDbDocReqRespTraceRepo : LiteDbBase<DocReqRespTraceDao, DocReqRespTraceDto> {


    public LiteDbDocReqRespTraceRepo(LiteDatabase db) : base(db) {

    }

    public override DocReqRespTraceDao ToDao(DocReqRespTraceDto source) {
        if (source is null)
            return null;

        return new DocReqRespTraceDao {
            DocId = source.DocId,
            LastUpdated = source.LastUpdated,
            StatusCode = source.StatusCode,
            Timestamp = source.Timestamp,
            ElapsedMsec = source.ElapsedMsec,
            RequestBody = source.RequestBody,
            RequestUri = source.RequestUri,
            RequestHeaders = source.RequestHeaders,
            ResponseHeaders = source.ResponseHeaders,
            ResponseBody = source.ResponseBody,
            RequestMethod = source.RequestMethod

        };
    }

    public override DocReqRespTraceDto ToDto(DocReqRespTraceDao source) {
        if (source is null)
            return null;

        return new DocReqRespTraceDto {
            DocId = source.DocId,
            LastUpdated = source.LastUpdated,
            StatusCode = source.StatusCode,
            Timestamp = source.Timestamp,
            ElapsedMsec = source.ElapsedMsec,
            RequestBody = source.RequestBody,
            RequestUri = source.RequestUri,
            RequestHeaders = source.RequestHeaders,
            ResponseHeaders = source.ResponseHeaders,
            ResponseBody = source.ResponseBody,
            RequestMethod = source.RequestMethod
        };
    }

    // public void Upsert(string id, DocReqRespTraceDao entity) {
    //     var c = _db.GetCollection<DocReqRespTraceDao>();
    //     c.EnsureIndex(t => t.DocId);
    //     c.Upsert(id, entity);
    // }
}
