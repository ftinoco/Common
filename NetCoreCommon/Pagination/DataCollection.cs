using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace NetCoreCommon.Pagination
{
    /// <summary>
    /// Model to manage data collection
    /// </summary>
    /// <typeparam name="TData">Data type</typeparam>
    public class DataCollection<TData>
    {
        public bool HasItems
        {
            get
            {
                return Items != null && Items.Any();
            }
        }

        [JsonPropertyName("items")]
        public IEnumerable<TData> Items { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("page")]
        public int Page { get; set; }
        [JsonPropertyName("pages")]
        public int Pages { get; set; }
    }
}
