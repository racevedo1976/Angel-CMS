using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Angelo.Connect.Web.UI.Search.Components;

namespace Angelo.Connect.Web.UI.Search.TagHelpers
{
    [HtmlTargetElement("searchtools")]
    public class SearchInputTagHelper : TagHelper
    {
        private DefaultViewComponentHelper _componentHelper;

        [ViewContextAttribute]
        public ViewContext ViewContext { get; set; }

        public SearchInputTagHelper(IViewComponentHelper componentHelper)
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _componentHelper.Contextualize(ViewContext);

            output.TagName = "div";
            output.Content.SetHtmlContent(
                await _componentHelper.InvokeAsync<SearchInput>()
            );
        }

    }
}
