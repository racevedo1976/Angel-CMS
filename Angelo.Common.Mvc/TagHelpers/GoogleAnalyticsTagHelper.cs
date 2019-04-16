using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Angelo.Common.Mvc.TagHelpers
{
    [HtmlTargetElement("GoogleAnalytics")]

    public class GoogleAnalyticsTagHelper : TagHelper
    {
       
        public string GoogleTrackingId { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

          string googleTranslateCode = " " +
$"<script async src='https://www.googletagmanager.com/gtag/js?id={GoogleTrackingId}'></script>" +
"<script>" +
"        window.dataLayer = window.dataLayer || [];" +
"        function gtag() { dataLayer.push(arguments); }" +
"        gtag('js', new Date());" +
"" +
$"        gtag('config', '{GoogleTrackingId}');" +
"</script>" +
"";

            if (!string.IsNullOrEmpty(GoogleTrackingId))
            {
                output.TagName = "";
                output.Content.SetHtmlContent(googleTranslateCode);
            }
        }
    }
}
