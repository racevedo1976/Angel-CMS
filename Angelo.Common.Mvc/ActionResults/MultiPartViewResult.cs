using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Routing;


namespace Angelo.Common.Mvc
{
    /// <summary>
    /// Use this class with Angelo AJAX calls to return multiple views or sections withing a single AJAX call.
    /// </summary>
    public class MultiPartViewResult : Microsoft.AspNetCore.Mvc.ActionResult, IActionResult
    {
        const string AJAX_DATA_SECTION_TAG_NAME = "ajax_data_section";

        private class AjaxDataPart
        {
            public string Target;
            public string Data;
            public string Mode;
        }

        private List<AjaxDataPart> _parts;
        private Controller _controller;
        ICompositeViewEngine _viewEngine;

        /// <summary>
        /// Use this class with Angelo AJAX calls to return multiple views or sections withing a single AJAX call.
        /// </summary>
        /// <param name="controller">The Controller that will processing this ActionResult.</param>
        public MultiPartViewResult(Controller controller)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            _parts = new List<AjaxDataPart>();
            _controller = controller;

            var serviceProvider = controller.HttpContext.RequestServices;
            _viewEngine = serviceProvider.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            if (_viewEngine == null)
                throw new Exception("Unable to obtain a valid view engine from service provider.");
        }

        protected string RenderViewAsString(string partialViewName, object model)
        {
            //viewName = viewName ?? _controller.ControllerContext.ActionDescriptor.ActionName;
            _controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                IView view = _viewEngine.FindView(_controller.ControllerContext, partialViewName, false).View;
                if (view == null)
                    throw new NullReferenceException(string.Format("Unable to find partial view: {0}", partialViewName));
                ViewContext viewContext = new ViewContext(_controller.ControllerContext, view, _controller.ViewData, _controller.TempData, sw, new HtmlHelperOptions());

                view.RenderAsync(viewContext).Wait();

                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Returns the list index of the item having the specified key value.
        /// <para>Returns -1 if the specified key is not found.</para>
        /// </summary>
        protected int IndexOfKey(string target)
        {
            var index1 = 0;
            var found = -1;
            while ((found == -1) && (index1 < _parts.Count))
            {
                if (_parts[index1].Target.Equals(target, StringComparison.OrdinalIgnoreCase))
                    found = index1;
                index1 = index1 + 1;
            }
            return found;
        }

        /// <summary>
        /// Adds the specified part data for the specified target key.
        /// A new item will be created if the specified target key is not found.
        /// </summary>
        protected void SetPart(string target, string data, string mode)
        {
            var newPart = new AjaxDataPart()
            {
                Target = target,
                Data = data,
                Mode = mode.ToUpper()
            };

            var index1 = IndexOfKey(target);
            if (index1 == -1)
                _parts.Add(newPart);
            else
                _parts[index1] = newPart;
        }

        protected string HtmlContentToString(IHtmlContent content)
        {
            using (StringWriter sw = new StringWriter())
            {
                content.WriteTo(sw, HtmlEncoder.Default);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Sets the content at the specified list position.
        /// <para/>- Use this method to add content from TagBuilder, HtmlString, or other objects implementing the IHtmlContent interface.
        /// <para/>- If an item at the specified target key does not exist, then a new item will be created.
        /// </summary>
        public MultiPartViewResult SetContent(string target, IHtmlContent content, string mode = "REPLACE")
        {
            SetPart(target, HtmlContentToString(content), mode);
            return this;
        }

        /// <summary>
        /// Clears the content at the specified target key.
        /// <para/>- Use this method if you do not want the section at the specified target key to be updated.
        /// </summary>
        public MultiPartViewResult ClearContent(string target)
        {
            var index1 = IndexOfKey(target);
            if (index1 > -1)
                _parts.RemoveAt(index1);
            return this;
        }

        /// <summary>
        /// Returns the content at the specified list position.
        /// </summary>
        public IHtmlContent GetContent(string target)
        {
            var index1 = IndexOfKey(target);
            if (index1 == -1)
                return new HtmlString("");
            else
                return new HtmlString(_parts[index1].Data);
        }

        /// <summary>
        /// Sets the partial view at the specified target key.
        /// <para/>- If an item at the specified target key does not exist, then a new item will be created. 
        /// </summary>
        public MultiPartViewResult SetView(string target, string partialViewName, object model, string mode = "REPLACE")
        {
            SetPart(target, RenderViewAsString(partialViewName, model), mode);
            return this;
        }

        /// <summary>
        /// Sets the partial view at the specified target key.
        /// <para/>- If an item at the specified target key does not exist, then a new item will be created. 
        /// </summary>
        public MultiPartViewResult SetView(string target, PartialViewResult view, string mode = "REPLACE")
        {
            SetView(target, view.ViewName, view.Model, mode);
            return this;
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

            using (StringWriter sw = new StringWriter())
            {
                TagBuilder separatorTag;
                var count = _parts.Count;
                for (var i = 0; i < count; i++)
                {
                    separatorTag = new TagBuilder(AJAX_DATA_SECTION_TAG_NAME);
                    separatorTag.Attributes.Add("target", _parts[i].Target);
                    separatorTag.Attributes.Add("mode", _parts[i].Mode);
                    separatorTag.InnerHtml.AppendHtml(_parts[i].Data);
                    separatorTag.WriteTo(sw, HtmlEncoder.Default);
                }
                var data = System.Text.Encoding.UTF8.GetBytes(sw.GetStringBuilder().ToString());
                context.HttpContext.Response.ContentLength = data.Length;
                context.HttpContext.Response.ContentType = "text/html";
                await context.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);
                await context.HttpContext.Response.Body.FlushAsync();
            }
        }

        //// https://www.simple-talk.com/dotnet/asp.net/revisiting-partial-view-rendering-in-asp.net-mvc/
        //public override async Task ExecuteResultAsync(ActionContext context)
        //{
        //    if (context == null)
        //        throw new ArgumentNullException(nameof(context));
        //    byte[] ajaxDataSeparator = System.Text.Encoding.UTF8.GetBytes(AJAX_DATA_SEPARATOR);
        //    int count = _actionResults.Count();
        //    for (var i = 0; i < count; i++)
        //    {
        //        if (i > 0)
        //        {
        //            await context.HttpContext.Response.Body.WriteAsync(ajaxDataSeparator, 0, ajaxDataSeparator.Length);
        //        }
        //        if (_actionResults[i] != null)
        //            await _actionResults[i].ExecuteResultAsync(context);
        //    }
        //}

    }
}

