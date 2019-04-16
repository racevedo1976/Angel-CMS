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

using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering.Components;

namespace Angelo.Connect.Rendering.TagHelpers
{
    [HtmlTargetElement("zone")]
    public class ContentZoneTagHelper : TagHelper
    {
        private DefaultViewComponentHelper _componentHelper;
        private SiteContext _siteContext;

        [HtmlAttributeName("name")]
        public string ZoneName { get; set; }

        [HtmlAttributeName("id")]
        public string ParentId { get; set; }

        [HtmlAttributeName("class")]
        public string ZoneClass { get; set; }

        [ViewContextAttribute] // so razor will inject
        public ViewContext ViewContext { get; set; }

        public ContentZoneTagHelper(IViewComponentHelper componentHelper, SiteContext siteContext)
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
            _siteContext = siteContext;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var treeContext = ViewContext.ViewData.GetTreeContext();    
            var attributes = context.AllAttributes;

            // Build default zone settings then update settings 
            // TODO: Extend with other options to better control node behavior 
            treeContext.Zone = new ZoneContext
            {
                Name = ZoneName,
                Class = ZoneClass,
                AllowContainers = false,
                AllowPadding = true
            };


            if (treeContext.NodeId == null && treeContext.AllowContainers)
            {
                // for root nodes, the default for allowing containers should be same as tree context
                treeContext.Zone.AllowContainers = treeContext.AllowContainers;

                // unless specified otherwise by the zone
                if(attributes.ContainsName("allow-containers"))
                {
                    treeContext.Zone.AllowContainers = attributes["allow-containers"].Value.ToString().ToLower() == "true";
                    output.Attributes.Remove(attributes["allow-containers"]);
                }
            }

            if(attributes.ContainsName("allow-padding"))
            {
                treeContext.Zone.AllowPadding = attributes["allow-padding"].Value.ToString().ToLower() == "true";
                output.Attributes.Remove(attributes["allow-padding"]);
            }

            // Branch the ViewContext & update with new TreeContext
            var childViewData = new ViewDataDictionary(ViewContext.ViewData);
            var childViewContext = new ViewContext(ViewContext, ViewContext.View, childViewData, ViewContext.Writer);

            childViewData.SetTreeContext(treeContext);
            _componentHelper.Contextualize(childViewContext);


            // Render the zone component in the new context
            // TODO: Remove old css classes
            var zoneCssClass = ZoneClass + " content-zone cs-content-zone";


            if (treeContext.Editable)
            {
                zoneCssClass += " editable";
                output.Attributes.Add("data-tree-id", treeContext.TreeId);
                output.Attributes.Add("data-embedded", treeContext.Zone.Embedded ? "true" : "false");
                output.Attributes.Add("data-allow-containers", treeContext.Zone.AllowContainers ? "true" : "false");
                output.Attributes.Add("data-allow-padding", treeContext.Zone.AllowPadding ? "true" : "false");
            }

            output.Attributes.Add("class", zoneCssClass);
            output.Attributes.Add("name", ZoneName);
            output.TagName = "div";

            output.Content.AppendHtml(
                await _componentHelper.InvokeAsync<ContentZone>(new
                {
                    zone = ZoneName,
                    tree = treeContext.TreeId,
                    node = treeContext.NodeId
                })
            );
        }

    }
}
