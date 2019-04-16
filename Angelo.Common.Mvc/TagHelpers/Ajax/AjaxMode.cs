using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Mvc.TagHelpers
{
    public enum AjaxMode
    {
        Replace,        
        Prepend,
        Append,
        InsertAfter,
        InsertBefore
    }

    public static class AjaxModeExtensions
    {
        public static string ToStringValue(this AjaxMode mode)
        {
            switch (mode)
            {
                case AjaxMode.Append:
                    return "append";
                case AjaxMode.Prepend:
                    return "prepend";
                case AjaxMode.Replace:
                    return "replace";
                case AjaxMode.InsertAfter:
                    return "insertAfter";
                case AjaxMode.InsertBefore:
                    return "insertBefore";
                default:
                    return "replace";
            }
        }
    }
}
