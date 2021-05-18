using System.Net;
using System.Threading.Tasks;

namespace NetCoreCommon
{
    public interface IResult<out TData> : IBaseResult
    {
        TData Data { get; }
    }

    public class Result<TData> : BaseResult, IResult<TData>
    {
        public Result() { }

        public TData Data { get; set; }

        public static new Task<Result<TData>> FailAsync()
        {
            return Task.FromResult(new Result<TData>() { ResponseCode = HttpStatusCode.InternalServerError });
        }

        public static new Task<Result<TData>> FailAsync(string message)
        {
            return Task.FromResult(new Result<TData>() { ResponseCode = HttpStatusCode.InternalServerError, Message = message });
        }

        public static new Task<Result<TData>> NotFoundAsync()
        {
            return Task.FromResult(new Result<TData>() { ResponseCode = HttpStatusCode.NotFound });
        }

        public static new Task<Result<TData>> NotFoundAsync(string message)
        {
            return Task.FromResult(new Result<TData>() { ResponseCode = HttpStatusCode.NotFound, Message = message });
        }

        public static new Task<Result<TData>> BadRequestAsync()
        {
            return Task.FromResult(new Result<TData>() { ResponseCode = HttpStatusCode.BadRequest });
        }

        public static new Task<Result<TData>> BadRequestAsync(string message)
        {
            return Task.FromResult(new Result<TData>() { ResponseCode = HttpStatusCode.BadRequest, Message = message });
        }

        public static Task<Result<TData>> SuccessAsync(TData data)
        {
            return Task.FromResult(new Result<TData>() { Data = data, ResponseCode = HttpStatusCode.OK });
        }

        public static Task<Result<TData>> SuccessAsync(TData data, string message)
        {
            return Task.FromResult(new Result<TData>() { Data = data, ResponseCode = HttpStatusCode.OK, Message = message });
        }
    }
}
