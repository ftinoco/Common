using System.Text.Json.Serialization;

namespace NetCoreCommon.Pagination
{
    /// <summary>
    /// Model to manage filter page
    /// </summary>
    public class LazyPageFilter
    {
        /// <summary>
        /// Value of filter
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// Match mode of filter: "equals", "contains", "EndsWith", "StartsWith"
        /// </summary>
        [JsonPropertyName("matchMode")]
        public string MatchMode { get; set; }
    }
}