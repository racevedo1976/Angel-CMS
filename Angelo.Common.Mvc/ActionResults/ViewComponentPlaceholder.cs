using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Common.Mvc.ActionResults
{
    /// <summary>
    /// Use this class when you want to return a placeholder or empty component instead of a view.
    /// You would return this class within your ViewComponent.Invoke method of your ViewComponent controller.
    /// Note: You will be able to call the component's invoke javascript function once this placeholder has been added to the response.
    ///
    /// ex:
    ///   $("#nameOfYourComponent").component().invoke();
    ///   
    /// </summary>
    public class ViewComponentPlaceholder : IViewComponentResult
    {
        public void Execute(ViewComponentContext context)
        {
            var cid = (string)(context.ViewData["cid"] ?? "");
            var divTag = new TagBuilder("div");
            divTag.Attributes.Add("id", cid);
            divTag.WriteTo(context.Writer, context.HtmlEncoder);
        }

        public Task ExecuteAsync(ViewComponentContext context)
        {
            return Task.Run(() => Execute(context));
        }
    }
}