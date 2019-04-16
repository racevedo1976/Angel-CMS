using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;
using Angelo.Connect.UI.Components;

namespace Angelo.Connect.Rendering.TagHelpers
{
    [HtmlTargetElement("pagetools")]
    public class ContentPageToolbarTagHelper : TagHelper
    {
        private DefaultViewComponentHelper _componentHelper;

        [ViewContextAttribute]
        public ViewContext ViewContext { get; set; }

        public ContentPageToolbarTagHelper(IViewComponentHelper componentHelper)
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var renderingContext = ViewContext.ViewData.GetRenderingContext() as ShellContext;

            _componentHelper.Contextualize(ViewContext);

            output.TagName = "div";

            if(renderingContext.Toolbar?.ComponentType != null)
            {
                IHtmlContent toolbarContent = null;
                var component = renderingContext.Toolbar.ComponentType;
                var arguments = renderingContext.Toolbar.ComponentArguments;
             
                if(arguments != null)
                    toolbarContent = await _componentHelper.InvokeAsync(component, arguments);
                else
                    toolbarContent = await _componentHelper.InvokeAsync(component);

                // TODO: Remove old css classes
                output.Attributes.Add("class", "page-tools cs-page-tools");

                output.Content.SetHtmlContent(toolbarContent);
            }          
          
        }

    }
}
