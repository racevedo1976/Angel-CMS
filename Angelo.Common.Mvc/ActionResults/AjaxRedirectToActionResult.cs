using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Angelo.Common.Mvc
{
    public class AjaxRedirectToActionResult : ActionResult, IActionResult
    {
        const string AJAX_DATA_REDIRECT_TAG_NAME = "ajax_data_redirect";

        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public object RouteValues { get; set; }


        public AjaxRedirectToActionResult(string actionName, string controllerName, object routeValues)
        {
            ActionName = actionName;
            ControllerName = controllerName;
            RouteValues = routeValues;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var task = this.ExecuteResultAsync(context);
            task.RunSynchronously();
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var scheme = context.HttpContext.Request.Scheme;
            var helper = new UrlHelper(context);
            var redirectUrl = helper.Action(ActionName, ControllerName, RouteValues, scheme);
            using (StringWriter sw = new StringWriter())
            {
                var redirectTag = new TagBuilder(AJAX_DATA_REDIRECT_TAG_NAME);
                redirectTag.Attributes.Add("url", redirectUrl);
                redirectTag.WriteTo(sw, HtmlEncoder.Default);
                var data = System.Text.Encoding.UTF8.GetBytes(sw.GetStringBuilder().ToString());
                context.HttpContext.Response.ContentLength = data.Length;
                context.HttpContext.Response.ContentType = "text/html";
                await context.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);
                await context.HttpContext.Response.Body.FlushAsync();
            }
        }
    }
}
