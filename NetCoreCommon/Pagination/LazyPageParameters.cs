using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetCoreCommon.Pagination
{
    public enum SortOrder
    {
        DESC = -1,
        ASC = 1
    }

    public class LazyPageFilter
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("matchMode")]
        public string MatchMode { get; set; }
    }

    public class LazyPageParameters
    {
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        [JsonPropertyName("sortField")]
        public string SortField { get; set; }
        [JsonPropertyName("sortOrder")]
        public SortOrder SortOrder { get; set; }
        [JsonPropertyName("filters")]
        public Dictionary<string, LazyPageFilter> Filters { get; set; }
    }
}
