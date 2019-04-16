using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Angelo.Connect.UI.Components;

namespace Angelo.Connect.Rendering.TagHelpers
{
    [HtmlTargetElement("poweredby")]
    public class PoweredByTagHelper : TagHelper
    {
        private DefaultViewComponentHelper _componentHelper;

        [ViewContextAttribute]
        public ViewContext ViewContext { get; set; }

        public PoweredByTagHelper(IViewComponentHelper componentHelper)
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _componentHelper.Contextualize(ViewContext);

            output.TagName = "div";
            output.Attributes.Add("class", "powered-by");
            output.Content.AppendHtml(
                await _componentHelper.InvokeAsync<GenericView>(new
                {
                    view = "/UI/Views/Rendering/Other/PoweredBy.cshtml"
                })
            );
        }

    }
}
