using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;

namespace Angelo.Common.Extensions
{
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Use this extension method to copy/map the items from the fromList to this collection.
        /// Notes: 
        /// - Only Collection objects of types derived from IMappableViewModel will be able to use this method.
        /// - Any existing items in this collection will be removed by this method.
        /// - This method will not work with IEnumeral lists.  Use ICollection or IList instead. 
        /// </summary>
        public static ICollection<VM> CopyFrom<VM, T>(this ICollection<VM> toList, IEnumerable<T> fromList) where VM : IMappableViewModel, new() where T : class
        {
            toList.Clear();
            foreach (var item in fromList ?? new T[0])
            {
                var newItem = new VM();
                newItem.CopyFrom(item);
                toList.Add(newItem);
            }
            return toList;
        }

        /// <summary>
        /// Use this extension method to copy/map the items from the this collection to the toList.
        /// Notes: 
        /// - Only Collection objects of types derived from IMappableViewModel will be able to use this method.
        /// - Any existing items in the toList collection will be removed by this method.
        /// - This method will not work with an IEnumeral toList.  Use ICollection or IList instead. 
        /// </summary>
        public static ICollection<T> CopyTo<VM, T>(this IEnumerable<VM> fromList, ICollection<T> toList) where VM : IMappableViewModel where T : class, new()
        {
            toList.Clear();
            foreach (var item in fromList ?? new VM[0])
            {
                var newItem = new T();
                item.CopyTo(newItem);
                toList.Add(newItem);
            }
            return toList;
        }

        /// <summary>
        /// Use this method to copy/map the contents from the fromPagedResult object into this instance of PageResult.
        /// Notes: 
        /// - Only PagedResult objects of types derived from IMappableViewModel will be able to use this method.
        /// - Any existing items in this collection will be removed by this method.
        /// </summary>
        public static PagedResult<VM> CopyFrom<VM, T>(this PagedResult<VM> toPagedResult, PagedResult<T> fromPagedResult) where VM : class, IMappableViewModel, new() where T : class
        {
            toPagedResult.Data = new List<VM>().CopyFrom(fromPagedResult.Data);
            toPagedResult.PageSize = fromPagedResult.PageSize;
            toPagedResult.PageCount = fromPagedResult.PageCount;
            toPagedResult.PageNumber = fromPagedResult.PageNumber;
            toPagedResult.ItemCount = fromPagedResult.ItemCount;
            return toPagedResult;
        }
    }
}


