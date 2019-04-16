using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Widgets
{
    public interface IWidgetService<TModel> : IWidgetService where TModel : class, IWidgetModel
    {
        TModel GetModel(string widgetId);
        TModel GetDefaultModel();

        void SaveModel(TModel model);
        void UpdateModel(TModel model);
        void DeleteModel(string widgetId);

        /// <summary>
        /// Creates & returns a new model from the supplied model. 
        /// The returned model should be persisted as a separate instance and contain a unique Id from the model supplied.
        /// </summary>
        /// <param name="model">The model to clone</param>
        /// <returns>A newly cloned model</returns>
        TModel CloneModel(TModel model);
    }
}
