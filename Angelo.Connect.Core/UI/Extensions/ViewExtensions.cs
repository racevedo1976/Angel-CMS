using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UI.Extensions
{
    public static class ViewExtensions
    {
        /*
        public static IHtmlContent Partial(this IHtmlHelper helper, GenericViewResult viewResult)
        {
            var mergedViewData = new ViewDataDictionary(helper.ViewData);

            foreach (var keyValuePair in viewResult.ViewData)
            {
                mergedViewData.Add(keyValuePair);
            }    

            return helper.Partial(viewResult.ViewPath, viewResult.ViewModel, mergedViewData);
        }
        */

        public static IHtmlContent Partial(this IHtmlHelper helper, GenericViewResult viewResult)
        {
            if (viewResult?.ViewData != null)
            {
                foreach (var item in viewResult.ViewData)
                {
                    if (helper.ViewData.ContainsKey(item.Key))
                        helper.ViewData[item.Key] = item.Value;

                    else
                        helper.ViewData.Add(item.Key, item.Value);
                }
            }

            return helper.Partial(viewResult.ViewPath, viewResult.ViewModel);
        }
    }
}