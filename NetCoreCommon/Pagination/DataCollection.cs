using System.Collections.Generic;
using System.Linq;

namespace NetCoreCommon.Pagination
{
    public class DataCollection<TData>
    {
        public bool HasItems
        {
            get
            {
                return Items != null && Items.Any();
            }
        }

        public IEnumerable<TData> Items { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
    }
}
