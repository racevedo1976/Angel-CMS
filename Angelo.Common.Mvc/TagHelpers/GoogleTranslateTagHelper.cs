using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Angelo.Common.Mvc.TagHelpers
{

    [HtmlTargetElement("GoogleTranslate")]
    
    public class GoogleTranslateTagHelper : TagHelper
    {
        private string googleTranslateCode = @" 

<div id='google_translate_element'></div>
<script>
        function googleTranslateElementInit()
        {
            new google.translate.TranslateElement({
                pageLanguage: 'en',
                autoDisplay: false,
                layout: google.translate.TranslateElement.InlineLayout.SIMPLE
            }, 'google_translate_element');
        }
</script>
<script src='//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit'></script>
";

        public bool EnableGoogleTranslate { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (EnableGoogleTranslate)
            {
                output.Content.SetHtmlContent(googleTranslateCode);
            }
        }
    }
}
