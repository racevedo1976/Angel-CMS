using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Angelo.Aegis.Messaging
{
    public class TemplateService
    {
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;

        public TemplateService(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider
        )
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public string Interpolate<TModel>(string template, TModel model)
        {

            var actionContext = GetActionContext();
            //var viewEngineResult = _viewEngine.FindView(actionContext, template, false);
            var viewEngineResult = _viewEngine.GetView(null, template, false);


            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException(string.Format("Couldn't find view template '{0}'", template));
            }

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var tempData = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
                var viewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    viewData,
                    tempData,
                    output, 
                    new HtmlHelperOptions()
                );

                view.RenderAsync(viewContext).GetAwaiter().GetResult();

                return output.ToString();
            }
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }

}
