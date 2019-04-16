using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Angelo.Connect.Extensions
{
    public static class AngeloStringExtensions
    {
        public static string FormatTenantKey(this string tenantKey)
        {
            char[] arr = tenantKey.ToLower().ToCharArray();
            arr = Array.FindAll<char>(arr, (c => char.IsLetterOrDigit(c) || c == '-'));
            return new string(arr);
        }
    }
}

