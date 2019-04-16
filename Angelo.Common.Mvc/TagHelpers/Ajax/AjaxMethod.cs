using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Mvc.TagHelpers
{
    public enum AjaxMethod
    {
        Get,        
        Post,
        Put,
        Delete
    }

    public static class AjaxMethodExtensions
    {
        public static string ToStringValue(this AjaxMethod method)
        {
            switch (method)
            {
                case AjaxMethod.Get:
                    return "GET";
                case AjaxMethod.Post:
                    return "POST";
                case AjaxMethod.Put:
                    return "PUT";
                case AjaxMethod.Delete:
                    return "DELETE";
                default:
                    return "GET";
            }
        }
    }
}
