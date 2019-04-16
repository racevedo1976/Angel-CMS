using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Angelo.Common.Extensions;
using Angelo.Connect.Services;
using Angelo.Connect.UI.Components;

namespace Angelo.Connect.Rendering.TagHelpers
{
    [HtmlTargetElement("page")]
    public class ContentPageTagHelper : TagHelper
    {
        private DefaultViewComponentHelper _componentHelper;
        private IRazorViewEngine _viewEngine;
        private ContentManager _contentManager;

        [ViewContextAttribute] 
        public ViewContext ViewContext { get; set; }

        public ContentPageTagHelper
        (
            IViewComponentHelper componentHelper,
            ContentManager contentManager
        )
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
            _contentManager = contentManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {             
            var attributes = context.AllAttributes;
            var renderingContext = ViewContext.ViewData.GetRenderingContext();

            if (renderingContext == null)
                throw new NullReferenceException("Cannot Render Main Content: Null Rendering Context");

            // Branch ViewContext as the inner Page will have a new Tree Context
            var childViewData = new ViewDataDictionary(ViewContext.ViewData);
            var childViewContext = new ViewContext(ViewContext, ViewContext.View, childViewData, ViewContext.Writer);

            // Establish new tree context for access by child contexts
            var treeContext = new TreeContext
            {
                TreeId = renderingContext.ContentTreeId,
                Editable = renderingContext.ContentEditable,
                AllowContainers = true
            };

            // By default we will allow containers for immediate children of main content unless specifically turned off
            if (attributes.ContainsName("allow-containers"))
            {
                treeContext.AllowContainers = attributes["allow-containers"].Value.ToString().ToLower() == "true";
                output.Attributes.Remove(attributes["allow-containers"]);
            }

            // Reset TreeContext for descendent views
            childViewData.SetTreeContext(treeContext);

            // Contextualize after updating context(s)
            _componentHelper.Contextualize(childViewContext);

            // TODO: Remove old css classes
            output.Attributes.Add("class", "page-content content-tree cs-main-content cs-content-tree");

            // NOTE: Internal content IDs should only be exposed when in design mode for authorized users
            if (treeContext.Editable)
            {
                output.Attributes.Add("id", treeContext.TreeId);
            }
            else
            {
                output.Attributes.Add("id", "maincontent");
            }

            output.TagName = "div";
            output.Attributes.Add("role", "main");

            output.Content.AppendHtml(
                await _componentHelper.InvokeAsync<GenericView>(new {
                    view = renderingContext.ContentViewPath,
                    model = renderingContext.ContentViewModel
                })
            );
        }

    }
}
