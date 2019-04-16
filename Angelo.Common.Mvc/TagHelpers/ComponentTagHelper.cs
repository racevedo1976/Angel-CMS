using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace Angelo.Common.Mvc.TagHelpers
{
    [HtmlTargetElement("component")]
    public class ComponentTagHelper : TagHelper
    {
        private DefaultViewComponentHelper _componentHelper;

        [HtmlAttributeName("id")]
        public string ComponentId { get; set; }

        [HtmlAttributeName("type")]
        public string ComponentType { get; set; }

        [HtmlAttributeName("params")]
        public object ComponentParams { get; set; }

        [HtmlAttributeName("args", DictionaryAttributePrefix = "arg-")]
        public IDictionary<string, object> ComponentArgs { get; set; }

        [HtmlAttributeName("lazy")]
        public bool Lazyload { get; set; } = false;

        [ViewContextAttribute] // so razor will inject
        public ViewContext ViewContext { get; set; }

        public ComponentTagHelper(IViewComponentHelper componentHelper)
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
            ComponentArgs = new Dictionary<string, object>();
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _componentHelper.Contextualize(ViewContext);

            if (ComponentId == null)
                ComponentId = ComponentType;

            if (ComponentArgs != null && ComponentArgs.Count > 0)
                ComponentParams = ToExpando(ComponentArgs);

            var componentData = new
            {
                id = ComponentId,
                type = ComponentType,
                @params = ComponentParams
            };

            ViewContext.ViewData["cid"] = ComponentId;

            output.Attributes.Add("id", ComponentId + "_component");
            output.Attributes.Add("data-component", JsonConvert.SerializeObject(componentData));

            if (!Lazyload)
            {
                output.Content.AppendHtml(
                    await _componentHelper.InvokeAsync(ComponentType, ComponentParams)
                );
            }
            else
            {
                output.Content.AppendHtml($"<div id=\"{ComponentId}\"></div>");
            }
        }

        public static ExpandoObject ToExpando(IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var entry in dictionary)
            {
                expando.Add(entry.Key, entry.Value);
            }

            return (ExpandoObject)expando;
        }
    }
}
