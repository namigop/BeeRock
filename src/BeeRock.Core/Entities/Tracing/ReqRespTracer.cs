using System.Diagnostics;
using BeeRock.Core.Dtos;
using BeeRock.Core.Interfaces;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;

namespace BeeRock.Core.Entities.Tracing;

public class ReqRespTracer {
    private const int MAX_CACHE_SIZE = 1000;
    private const int BATCH_SIZE = 10;
    private const int MAX_DB_TRACE_COUNT = 5000;

    private IDocReqRespTraceRepo _repo;

    private List<DocReqRespTraceDto> _cache = new(MAX_CACHE_SIZE);
    private TaskCompletionSource<int> _bufferingCs = new TaskCompletionSource<int>();
    private int _readPos = 0;
    private int _writePos = 0;
    private static object key = new object();

    public event EventHandler<DocReqRespTraceDto> Traced;

    private ReqRespTracer() {
        _repo = null;
        ClearCache();
    }

    public static Lazy<ReqRespTracer> Instance = new(() => new ReqRespTracer());

    private void ClearCache() {
        _cache.Clear();
        for (int i = 0; i < MAX_CACHE_SIZE; i++)
            _cache.Add(null);
    }

    public void Setup(IDocReqRespTraceRepo repo) {
        _repo ??= repo;

        //Start the background task to read then write to DB
        Task.Run(async () => {
            while (true) {
                var targetPos = await _bufferingCs.Task;
                this._bufferingCs = new TaskCompletionSource<int>();
                Flush(targetPos);
            }
        });

        Task.Run(Cleanup);
    }

    public void Flush(int targetPos) {
        var delta = GetDelta(targetPos, _readPos);
        for (int i = 0; i <= delta; i++) {
            var dto = _cache[_readPos];
            _cache[_readPos] = null;
            Save(dto);
            this._readPos = MoveForward(_readPos, MAX_CACHE_SIZE);
        }
    }

    public void FlushAll() {
        this.Flush(this._writePos);
    }
    public List<DocReqRespTraceDto> GetAll() {
        Requires.NotNull(_repo, "repo");
        return
            _repo.All()
                .Concat(_cache)
                .Where(c => c != null)
                .OrderBy(c => c.Timestamp)
                .ToList();
    }

    private void Save(DocReqRespTraceDto dto) {
        if (dto != null) {
            _repo.Create(dto);
        }
    }

    private static int MoveForward(int x, int max) {
        if (x == max - 1)
            return 0;
        return x + 1;
    }

    private static int GetDelta(int writePos, int readPos) {
        if (writePos > readPos)
            return writePos - readPos;

        if (writePos == readPos)
            return 0;

        return ((MAX_CACHE_SIZE - 1) - readPos) + writePos;
    }

    public void Trace(DocReqRespTraceDto dto) {
        lock (key) {
            _cache[_writePos] = dto;
            if (GetDelta(_writePos, _readPos) >= BATCH_SIZE) {
                _bufferingCs.SetResult(_writePos);
            }

            _writePos = MoveForward(_writePos, MAX_CACHE_SIZE);
        }

        this.Traced?.Invoke(this, dto);
    }

    public void ClearAll() {
        lock (key) {
            this.ClearCache();
            this._writePos = 0;
            this._readPos = 0;
            _repo.DeleteAll();
        }
    }

    public void Cleanup() {
        var count = _repo.Count();
        if ( count > MAX_DB_TRACE_COUNT) {
            var toRemove = _repo.All().Take(count - MAX_DB_TRACE_COUNT);
            foreach (var i in toRemove)
                _repo.Delete(i.DocId);

            _repo.Shrink();
        }


    }
}
