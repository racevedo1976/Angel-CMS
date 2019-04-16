using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Angelo.Common.Mvc.TagHelpers
{

    public class ColTagHelper : TagHelper
    {
        public int Size { get; set; } = 1;
        public int? Sm { get; set; }
        public int? Xs { get; set; }

        bool _xs1, _xs2;

        [HtmlAttributeNotBound]
        public bool? Xs1 { get {  return null; } set { _xs1 = true; } } 
        [HtmlAttributeNotBound]
        public bool? Xs2 { get { return null; } set { _xs2 = true; } } 

        public ColTagHelper() { }
        public ColTagHelper(int size) { Size = size; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var defaultCssPrefix = "col-md";
            var columnCssClasses = new List<string>();

            if (context.AllAttributes.ContainsName("class"))
                columnCssClasses.Add(context.AllAttributes["class"].Value.ToString());

            Action<string, string> MapAttributeToCssClass = (attr, css) =>
            {
                if (context.AllAttributes.ContainsName(attr))
                    columnCssClasses.Add(css);
            };

            MapAttributeToCssClass("sm1", "col-sm-1");
            MapAttributeToCssClass("sm2", "col-sm-2");
            MapAttributeToCssClass("sm3", "col-sm-3");
            MapAttributeToCssClass("sm4", "col-sm-4");
            MapAttributeToCssClass("sm5", "col-sm-5");
            MapAttributeToCssClass("sm6", "col-sm-6");
            MapAttributeToCssClass("sm7", "col-sm-7");
            MapAttributeToCssClass("sm8", "col-sm-8");
            MapAttributeToCssClass("sm9", "col-sm-9");
            MapAttributeToCssClass("sm10", "col-sm-10");
            MapAttributeToCssClass("sm11", "col-sm-11");
            MapAttributeToCssClass("sm12", "col-sm-12");
            MapAttributeToCssClass("xs1", "col-xs-1");
            MapAttributeToCssClass("xs2", "col-xs-2");
            MapAttributeToCssClass("xs3", "col-xs-3");
            MapAttributeToCssClass("xs4", "col-xs-4");
            MapAttributeToCssClass("xs5", "col-xs-5");
            MapAttributeToCssClass("xs6", "col-xs-6");
            MapAttributeToCssClass("xs7", "col-xs-7");
            MapAttributeToCssClass("xs8", "col-xs-8");
            MapAttributeToCssClass("xs9", "col-xs-9");
            MapAttributeToCssClass("xs10", "col-xs-10");
            MapAttributeToCssClass("xs11", "col-xs-11");
            MapAttributeToCssClass("xs12", "col-xs-12");

            MapAttributeToCssClass("sm0", "hidden-sm");
            MapAttributeToCssClass("sm0", "hidden-xs");
            MapAttributeToCssClass("xs0", "hidden-xs");



            if (columnCssClasses.Any(x => x.StartsWith("col-sm")))
                defaultCssPrefix = "col-md";

            columnCssClasses.Add($"{defaultCssPrefix}-{Size}");
            output.Attributes.SetAttribute("class", String.Join(" ", columnCssClasses));
            output.TagName = "div";
            
            output.Content.SetHtmlContent(
                output.GetChildContentAsync().Result.GetContent()
            );

            await Task.FromResult(0);
        }
    }
    public class Col1TagHelper : ColTagHelper { public Col1TagHelper() : base(1) { } }
    public class Col2TagHelper : ColTagHelper { public Col2TagHelper() : base(2) { } }
    public class Col3TagHelper : ColTagHelper { public Col3TagHelper() : base(3) { } }
    public class Col4TagHelper : ColTagHelper { public Col4TagHelper() : base(4){} }
    public class Col5TagHelper : ColTagHelper { public Col5TagHelper() : base(5) { } }
    public class Col6TagHelper : ColTagHelper { public Col6TagHelper() : base(6) { } }
    public class Col7TagHelper : ColTagHelper { public Col7TagHelper() : base(7) { } }
    public class Col8TagHelper : ColTagHelper { public Col8TagHelper() : base(8) { } }
    public class Col9TagHelper : ColTagHelper { public Col9TagHelper() : base(9) { } }
    public class Col10TagHelper : ColTagHelper { public Col10TagHelper() : base(10) { } }
    public class Col11TagHelper : ColTagHelper { public Col11TagHelper() : base(11) { } }
    public class Col12TagHelper : ColTagHelper { public Col12TagHelper() : base(12) { } }

}
