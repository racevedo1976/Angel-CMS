using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace Angelo.Common.Mvc.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-readonly")]
    public class ReadOnlyTagHelper : TagHelper
    {

        [HtmlAttributeName("asp-readonly")]
        public bool ReadOnly { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ReadOnly)
            {
                output.Attributes.SetAttribute("readonly", "readonly");
            }
            else
            {
                output.Attributes.Remove(new TagHelperAttribute("readonly"));
            }
        }
    }
}
