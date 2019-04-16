using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;

namespace Angelo.Common.Extensions
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Returns an asynchronous task that returns a PageResult containing the specified page of data.
        /// </summary>
        /// <param name="query">The Linq query defining the rows belonging to the resulting dataset.</param>
        /// <param name="pageNumber">The 1-based offset for the page of data (default = 1).</param>
        /// <param name="pageSize">The maximum number of records each page can hold (default = 20).</param>
        public static async Task<PagedResult<T>> PagedResultAsync<T>(this IQueryable<T> query, int pageNumber = 1, int pageSize = 20) where T : class
        {
            var pagedResult = new PagedResult<T>();
            await pagedResult.LoadPageAsync(query, pageNumber, pageSize);
            return pagedResult;
        }

        /// <summary>
        /// Returns a PageResult instance containing the specified page of data.
        /// </summary>
        /// <param name="list">A list of all of the entities.</param>
        /// <param name="pageNumber">The 1-based offset for the page of data (default = 1).</param>
        /// <param name="pageSize">The maximum number of records each page can hold (default = 20).</param>
        public static PagedResult<T> PagedResult<T>(this IEnumerable<T> list, int pageNumber = 1, int pageSize = 20) where T : class
        {
            var pagedResult = new PagedResult<T>();
            pagedResult.LoadPage(list, pageNumber, pageSize);
            return pagedResult;
        }

    }
}
