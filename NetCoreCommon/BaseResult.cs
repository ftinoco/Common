using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetCoreCommon
{
    public interface IBaseResult
    {
        string Message { get; set; }

        HttpStatusCode ResponseCode { get; set; }
    }

    public class BaseResult: IBaseResult
    {
        public BaseResult() { }
         
        public string Message { get; set; }

        public HttpStatusCode ResponseCode { get; set; }

        public static Task<IBaseResult> NotFoundAsync()
        {
            return Task.FromResult((IBaseResult)new BaseResult { ResponseCode = HttpStatusCode.NotFound });
        }

        public static Task<IBaseResult> NotFoundAsync(string message)
        {
            return Task.FromResult((IBaseResult)new BaseResult { ResponseCode = HttpStatusCode.NotFound, Message = message });
        }

        public static Task<IBaseResult> FailAsync()
        {
            return Task.FromResult((IBaseResult)new BaseResult { ResponseCode = HttpStatusCode.InternalServerError });
        }

        public static Task<IBaseResult> FailAsync(string message)
        {
            return Task.FromResult((IBaseResult)new BaseResult { ResponseCode = HttpStatusCode.InternalServerError, Message = message });
        }

        public static Task<IBaseResult> BadRequestAsync()
        {
            return Task.FromResult((IBaseResult)new BaseResult { ResponseCode = HttpStatusCode.BadRequest });
        }

        public static Task<IBaseResult> BadRequestAsync(string message)
        {
            return Task.FromResult((IBaseResult)new BaseResult { ResponseCode = HttpStatusCode.BadRequest, Message = message });
        }

        public static Task<IBaseResult> SuccessAsync()
        {
            return Task.FromResult((IBaseResult)new BaseResult { ResponseCode = HttpStatusCode.OK });
        }

        public static Task<IBaseResult> SuccessAsync(string message)
        {
            return Task.FromResult((IBaseResult)new BaseResult { ResponseCode = HttpStatusCode.OK, Message = message });
        }
    }
}
