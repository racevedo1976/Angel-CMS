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
using Angelo.Connect.UI.Components;

namespace Angelo.Connect.Rendering.TagHelpers
{
    [HtmlTargetElement("usertools")]
    public class UserToolsTagHelper : TagHelper
    {
        private DefaultViewComponentHelper _componentHelper;
        private string _viewPath = "/UI/Views/Rendering/UserToolsPreLogin.cshtml";

        [ViewContextAttribute]
        public ViewContext ViewContext { get; set; }

        public UserToolsTagHelper(IViewComponentHelper componentHelper, SiteContext siteContext)
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _componentHelper.Contextualize(ViewContext);
        
            if (ViewContext.HttpContext.User.Identity.IsAuthenticated)
                _viewPath = "/UI/Views/Rendering/UserToolsPostLogin.cshtml";

            output.TagName = "div";
            output.Content.SetHtmlContent(
               await _componentHelper.InvokeAsync<GenericView>(new {view = _viewPath})
           );
        }

    }
}
