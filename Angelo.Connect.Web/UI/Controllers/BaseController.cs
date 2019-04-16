using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

using Angelo.Connect.Models;
using Angelo.Common.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.Controllers
{
    public class BaseController : Controller
    {
        protected ILogger Logger { get; set; }

        public BaseController(ILogger logger)
        {
            Logger = logger;
        }


        //TODO: Create a normalized error model / view for ErrorView()
        protected ActionResult ErrorView()
        {
            Logger.LogError("Unspecified Error");
            return View("Error");
        }

        protected ActionResult ErrorView(string error)
        {
            Logger.LogError(error);
            return View("Error");
        }

        protected ActionResult ErrorView(ModelStateDictionary modelState)
        {
            Logger.LogError("ModelState Error", modelState);
            return View("Error");
        }

        protected ActionResult ErrorView(ServiceResult result)
        {
            Logger.LogError("ServiceResult Error", result.Errors);
            return View("Error");
        }

        protected MultiPartViewResult MultiPartView()
        {
            return new MultiPartViewResult(this);
        }

        protected AjaxRedirectToActionResult AjaxRedirectToAction(string actionName, object routeValues)
        {
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;
            return new AjaxRedirectToActionResult(actionName, controllerName, routeValues);
        }

        protected string Localize(string resourceName, string text)
        {
            var factory = HttpContext.RequestServices.GetService(typeof(IStringLocalizerFactory));
            if (factory != null)
            {
                var localizer = ((IStringLocalizerFactory)factory).Create(resourceName, null);
                if (localizer != null)
                    text = localizer.GetString(text);
            }
            return text;
        }

        // use in conjunction with the DropDown select list Html helper.
        protected ActionResult AjaxSelectList(SelectList selectList)
        {
            return new JsonResult(new { items = selectList });
        }

        protected ActionResult AccessDenied()
        {
            Logger.LogError("Access Denied.");
            return View("AccessDenied");
        }
    }
}
