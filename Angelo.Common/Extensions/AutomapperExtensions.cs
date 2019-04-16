using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AutoMapper.Extensions
{
    public static class AutoMapperExtensions
    {
        public static ICollection<TType> ProjectTo<TType>(this IEnumerable data)
        {
            if (data == null)
                return null;

            return QueryableExtensions.Extensions.ProjectTo<TType>(data.AsQueryable()).ToList();
        }

        public static TType ProjectTo<TType>(this object source)
        {
            if (source == null)
                return default(TType);

            return Mapper.Map<TType>(source);
        }

        public static TType Clone<TType>(this TType source)
        {
            if (source == null)
                return default(TType);

            return Mapper.Map<TType>(source);
        }
    }
}