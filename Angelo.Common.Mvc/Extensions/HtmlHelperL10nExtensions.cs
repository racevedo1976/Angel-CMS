using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Localization;


namespace Angelo.Common.Mvc
{
    public static class HtmlHelperL10nExtensions
    {
        /// <summary>
        /// Returns the property's ShortName display attribute or otherwise the Name display attribute
        /// </summary>
        /// <param name="helper">The current HtmlHelper instance</param>
        /// <param name="expression">The model expression</param>
        /// <returns></returns>
        public static IHtmlContent ShortNameFor<TModel,TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var modelExplorer = GetModelExplorer(helper, expression);
            var modelType = modelExplorer.Metadata.ContainerType;
            var propertyName = modelExplorer.Metadata.PropertyName;
            var propertyInfo = modelType.GetProperty(propertyName);
            var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            var shortNameText = propertyName;

            if (displayAttr != null)
            {
                shortNameText = displayAttr.GetShortName() ?? displayAttr.GetName();  
            }

            shortNameText = TryLocalizeString(helper, modelType, shortNameText);

            return new HtmlString(shortNameText);
        }

        /// <summary>
        /// Localizes strings by checking the current view resource file, then any ancestor 
        /// resource files, then finally the shared resource file
        /// </summary>
        /// <param name="helper">The current HtmlHelper instance</param>
        /// <param name="text">The text to translate</param>
        public static IHtmlContent Localize(this IHtmlHelper helper, string text)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var httpContext = helper.ViewContext?.HttpContext;
            var resource = GetViewResouceName(helper);
            var oldText = text;

            // first try to localize using using a localizer for the current view or ancestor
            while (oldText == text && resource != null)
            {
                text = TryLocalizeString(helper, resource, text);
                resource = GetParentResourceName(resource);
            }

            // otherwise try to localize from shared resources in Global
            if (text == oldText)
                text = TryLocalizeString(helper, "Global", text);

            // TODO: Port everything to just use strings rather than keys... much simpler
            if (text == oldText)
                text = TryLocalizeString(helper, "Strings", text);

            return new HtmlString(text);
        }

        public static IHtmlContent Localize(this IHtmlHelper helper, string text, object arg0)
        {
            text = Localize(helper, text).ToString();
            text = string.Format(text, arg0);

            return new HtmlString(text);
        }

        public static IHtmlContent Localize(this IHtmlHelper helper, string text, object arg0, object arg1)
        {
            text = Localize(helper, text).ToString();
            text = string.Format(text, arg0, arg1);

            return new HtmlString(text);
        }

        #region IHtmlHelper private helpers
        /// <summary>
        /// Obtains a model meta data explorer for the supplied expression
        /// </summary>
        private static ModelExplorer GetModelExplorer<TModel, TResult>(IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (expression == null) 
                throw new ArgumentNullException(nameof(expression)); 

            var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, helper.ViewData, helper.MetadataProvider);

            if (modelExplorer == null) 
                throw new InvalidOperationException("Failed to create ModelExplorer for expression " + expression.ToString()); 

            return modelExplorer; 
        }

        /// <summary>
        ///  Trys to localize using a IStringLocalizer of the ModelType supplied
        /// </summary>
        private static string TryLocalizeString(IHtmlHelper helper, Type modelType, string text)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (modelType == null)
                throw new ArgumentNullException(nameof(modelType));

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var httpContext = helper?.ViewContext?.HttpContext;

            if (httpContext != null)
            {
                var localizerType = typeof(IStringLocalizer<>).MakeGenericType(modelType);
                var localizerService = httpContext.RequestServices.GetService(localizerType);

                if (localizerService != null)
                    text = ((IStringLocalizer)localizerService).GetString(text);
            }
            return text;
        }

        /// <summary>
        ///  Trys to localize using a IStringLocalizer corresponding to the current view
        /// </summary>
        private static string TryLocalizeString(IHtmlHelper helper, string resourceName, string text)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (String.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(nameof(resourceName));

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var httpContext = helper.ViewContext?.HttpContext;
            if (httpContext != null)
            {
                var factory = httpContext.RequestServices.GetService(typeof(IStringLocalizerFactory));
                if (factory != null)
                {
                    var localizer = ((IStringLocalizerFactory)factory).Create(resourceName, null);
                    if (localizer != null)
                        text = localizer.GetString(text);
                }
            }  
            return text;
        }

        /// <summary>
        /// Gets the Resource Name correpsonding to the IHtmlHelper's current view
        /// </summary>
        private static string GetViewResouceName(IHtmlHelper helper)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            string baseName = null;

            if (helper.ViewContext?.View != null)
            {
                var view = helper.ViewContext.View as RazorView;
                var viewType = view.RazorPage.GetType();
                var keys = viewType.Name.Split('_');

                baseName = String.Join(".", keys.Skip(1).Take(keys.Count() - 2));
            }
            return baseName;
        }
        
        /// <summary>
        /// Slices the rightmost qualifier from a fully qualified resource name
        /// </summary>
        private static string GetParentResourceName(string resourceName)
        {
            if (String.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(nameof(resourceName));

            var keys = resourceName.Split('.');
            if (keys.Length <= 1)
                return null;

            return String.Join(".", keys.Take(keys.Count() - 1));
        }
        
        #endregion
    }
}
