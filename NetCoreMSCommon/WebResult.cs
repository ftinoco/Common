using System.Text.Json.Serialization;

namespace NetCoreMSCommon
{
    public class WebResult<TData> : BaseWebResult
    {
        public WebResult() : base() { }

        public WebResult(TData data) : base()
        {
            Data = data;
        }

        [JsonPropertyName("data")]
        /// <summary>
        /// Datos de la petición
        /// </summary>
        public TData Data { get; set; }
    }
}
