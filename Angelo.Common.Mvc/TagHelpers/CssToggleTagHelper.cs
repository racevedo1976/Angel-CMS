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
    
    
    [HtmlTargetElement(Attributes = "toggle-css")]
    public class CssToggleTagHelper : TagHelper
    {   
        public class CssToggleOptions
        {
            public string Target { get; set; }
            public bool Toggle { get; set; }
            public string On { get; set; }
            public string Off { get; set; }
        }

        [HtmlAttributeName("toggle-css")]
        public Action<CssToggleOptions> OptionConfig { get; set; }



        public override void Process(TagHelperContext context, TagHelperOutput output)
        {            
            if (OptionConfig != null)
            {
                var options = new CssToggleOptions();
                var classes = new List<string>();
                var data = new List<string>();

                OptionConfig.Invoke(options);

                // prepend any existing css classes
                if (context.AllAttributes.ContainsName("class"))
                    classes.Add(context.AllAttributes["class"].Value.ToString());

                if(String.IsNullOrEmpty(options.Target) || options.Target.ToLower() == "this" || options.Target.ToLower() == "$(this)")
                {
                    classes.Add(options.Toggle ? options.On : options.Off);
                    data.Add("this");
                }

                data.Add(options.On);
                data.Add(options.Off);

                output.Attributes.SetAttribute("class", String.Join(" ", classes));
                output.Attributes.SetAttribute("data-toggle-css", JsonConvert.SerializeObject(data));
            }
        }
    }
}
