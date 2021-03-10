using System.Net;
using System.Text.Json.Serialization;

namespace NetCoreMSCommon
{
    public class BaseWebResult
    {
        public BaseWebResult()
        {
            Message = " ";
            ResponseCode = HttpStatusCode.OK;
        }

        [JsonPropertyName("message")]
        /// <summary>
        /// Mensaje de respuesta de la petición
        /// </summary>
        public string Message { get; set; }

        [JsonPropertyName("responseCode")]
        /// <summary>
        /// Código de respuesta de la petición
        /// </summary>
        public HttpStatusCode ResponseCode { set; get; }
    }
}
