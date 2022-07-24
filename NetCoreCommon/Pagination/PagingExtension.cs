using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NetCoreCommon.Pagination
{
    public static class PagingExtension
    {
        /// <summary>
        /// Get data collection of items based on pagging params
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="query">Data collection</param>
        /// <param name="page">page number</param>
        /// <param name="take">page size</param>
        /// <returns></returns>
        public static async Task<DataCollection<T>> GetPagedAsync<T>(
            this IQueryable<T> query,
            int page,
            int take)
        {
            var originalPages = page;

            page--;

            if (page > 0) page *= take;

            var result = new DataCollection<T>
            {
                Items = await query.Skip(page).Take(take).ToListAsync(),
                Total = await query.CountAsync(),
                Page = originalPages
            };

            if (result.Total > 0) 
                result.Pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(result.Total) / take));
            
            return result;
        }
    }
}