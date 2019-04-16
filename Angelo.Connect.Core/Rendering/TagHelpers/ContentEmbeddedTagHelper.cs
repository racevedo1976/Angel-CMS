using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Angelo.Connect.Data;
using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering.Components;

namespace Angelo.Connect.Rendering.TagHelpers
{
    [HtmlTargetElement("content")]
    public class ContentEmbeddedTagHelper : TagHelper
    {
        private DefaultViewComponentHelper _componentHelper;
        private SiteContext _siteContext;
        private ConnectDbContext _connectDb;
       
        [HtmlAttributeName("name")]
        public string ContainerName { get; set; }

        [HtmlAttributeName("type")]
        public string WidgetType { get; set; }

        [HtmlAttributeName("model")]
        public string ModelName { get; set; }


        [ViewContextAttribute] // so razor will inject
        public ViewContext ViewContext { get; set; }

        public ContentEmbeddedTagHelper(
            IViewComponentHelper componentHelper, 
            SiteContext siteContext, 
            ConnectDbContext connectDb
        )
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
            _siteContext = siteContext;
            _connectDb = connectDb;
        }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var treeContext = ViewContext.ViewData.GetTreeContext();
            string optionalViewId = null;

            _componentHelper.Contextualize(ViewContext);

            // checking if view is specified (optional)
            if (context.AllAttributes.ContainsName("view"))
            {
                optionalViewId = context.AllAttributes["view"].Value.ToString();
            }

            // adding meta data for designer when in design mode
            if (treeContext.Editable)
            {
                output.Attributes.Add("data-tree-id", treeContext.TreeId);

                // TODO: Remove old css classes
                output.Attributes.Add("class", "content-zone content-zone-embedded cs-content-zone cs-content-zone-embedded editable");
            }
            else
            {
                output.Attributes.Add("class", "content-zone cs-content-zone");
            }

            output.Attributes.Add("name", ContainerName);
            output.TagName = "div";

            output.Content.AppendHtml(
                await _componentHelper.InvokeAsync<ContentEmbedded>(new
                {
                    tree = treeContext.TreeId,
                    container = ContainerName,
                    widgetType = WidgetType,
                    modelName = ModelName,
                    viewId = optionalViewId
                })
            );
        }

    }
}
