namespace NetCoreCommon.Pagination.LazyPageModel
{
    /// <summary>
    /// Represents a sorting parameter.
    /// </summary>
    internal class SortingInfo
    {
        /// <summary>
        /// The data field to be used for sorting.
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// A flag indicating whether data should be sorted in a descending order.
        /// </summary>
        public bool Desc { get; set; }
    }
}
