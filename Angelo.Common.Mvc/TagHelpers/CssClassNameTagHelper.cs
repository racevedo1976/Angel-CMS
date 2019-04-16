using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Angelo.Common.Mvc.TagHelpers
{
    // MDJ: Found how to do this via AspNetCore source code
    // https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.TagHelpers/AnchorTagHelper.cs

    /// <summary>
    /// Appends a Css Class to an elmenent based on a condition. 
    /// 
    /// Usage:  css-odd="index % 2 == 1"
    ///         css-values="new { even = (index % 2 == 0), odd = (index % 2 == 1)}"
    ///         
    /// </summary>
    [HtmlTargetElement(Attributes = "css-values")]
    [HtmlTargetElement(Attributes = "css-*")]
    public class CssClassNameTagHelper : TagHelper
    {   
        [HtmlAttributeName("css-values", DictionaryAttributePrefix = "css-")]
        public IDictionary<string, bool> CssValues { get; set; }

        public CssClassNameTagHelper()
        {
            CssValues = new Dictionary<string, bool>();
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (CssValues != null && CssValues.Count > 0)
            {
                var classes = new List<string>();

                // prepend any existing css classes
                if (context.AllAttributes.ContainsName("class"))
                    classes.Add(context.AllAttributes["class"].Value.ToString());

                // add classes whose condition evalated to true
                foreach (var condition in CssValues)
                {
                    if(condition.Value == true)
                        classes.Add(condition.Key);
                }
                output.Attributes.SetAttribute("class", String.Join(" ", classes));
            }
        }
    }
}
