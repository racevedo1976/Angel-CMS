using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Angelo.Common.Models
{
    /// <summary>
    /// Container used to hold a page of rows in a dataset.
    /// </summary>
    public class PagedResult<T> where T : class
    {
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public int ItemCount { get; set; }
        public IEnumerable<T> Data { get; set; }

        public PagedResult()
        {
            PageSize = 20;
            PageCount = 0;
            PageNumber = 1;
            ItemCount = 0;
            Data = new List<T>();
        }

        private async void CalculateCountsAsync(IQueryable<T> query)
        {
            ItemCount = await query.CountAsync();
            PageCount = ItemCount / PageSize + ((ItemCount % PageSize) == 0 ? 0 : 1);
        }

        private void CalculateCounts(IEnumerable<T> list)
        {
            ItemCount = list.Count();
            PageCount = ItemCount / PageSize + ((ItemCount % PageSize) == 0 ? 0 : 1);
        }

        private async Task GetResultSetAsync(IQueryable<T> query)
        {
            Data = await query.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();
        }

        private void GetResultSet(IEnumerable<T> list)
        {
            Data = list.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        }

        /// <summary>
        /// Loads the specified page of data from the query.
        /// </summary>
        /// <param name="query">The Linq query defining the rows belonging to the resulting dataset.</param>
        /// <param name="pageNumber">The 1-based offset for the page of data (default = 1).</param>
        /// <param name="pageSize">The maximum number of records each page can hold (default = 20).</param>
        public async Task LoadPageAsync(IQueryable<T> query, int pageNumber = 1, int pageSize = 20)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            CalculateCountsAsync(query);
            await GetResultSetAsync(query);
        }

        /// <summary>
        /// Loads the specified page of data from the query.
        /// </summary>
        /// <param name="list">The collection of data items.</param>
        /// <param name="pageNumber">The 1-based offset for the page of data (default = 1).</param>
        /// <param name="pageSize">The maximum number of records each page can hold (default = 20).</param>
        public void LoadPage(IEnumerable<T> list, int pageNumber = 1, int pageSize = 20)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            CalculateCounts(list);
            GetResultSet(list);
        }

    }
}
