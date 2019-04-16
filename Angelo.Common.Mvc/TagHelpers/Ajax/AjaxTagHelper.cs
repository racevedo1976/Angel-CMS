using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;

// For a good example of helpers, go to:
// https://wesleycabus.be/2015/10/writing-a-custom-taghelper-in-asp-net-5/

namespace Angelo.Common.Mvc.TagHelpers
{
    [HtmlTargetElement(Attributes = "ajax-target")]
    public class AjaxTagHelper : TagHelper
    {
        [HtmlAttributeName("ajax-method")]
        public AjaxMethod Method { get; set; } = AjaxMethod.Get;

        [HtmlAttributeName("ajax-mode")]
        public AjaxMode Mode { get; set; } = AjaxMode.Replace;

        [HtmlAttributeName("ajax-target")]
        public string Target { get; set; } = string.Empty;

        public AjaxTagHelper(IHtmlGenerator generator)
        {

        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //string content = output.GetChildContentAsync().Result.GetContent();
            //base.Process(context, output);

            if (!String.IsNullOrEmpty(Target))
            {
                output.Attributes.Add("data-ajax", "true");
                output.Attributes.Add("data-ajax-method", Method.ToStringValue());
                output.Attributes.Add("data-ajax-mode", Mode.ToStringValue());
                output.Attributes.Add("data-ajax-update", Target);
            }
        }
    }
}
