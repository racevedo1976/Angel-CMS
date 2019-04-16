using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Angelo.Connect.SlideShow.Extensions
{
    public static class TypeExtensions
    {
        public static ICollection<SelectListItem> GetItemsForEnum(this Type enumeration, string nullItem = null)
        {
            var start = nullItem == null ? Enumerable.Empty<SelectListItem>() : new[] { new SelectListItem() { Text = nullItem } };
            return start.Concat(
                    enumeration.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(x => new SelectListItem() { Text = x.Name, Value = ((int)x.GetValue(enumeration)).ToString() })
                )
                .ToArray();
        }
    }
}
