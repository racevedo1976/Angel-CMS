using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Widgets
{
    interface IWidgetExportService<TModel> where TModel : class, IWidgetModel
    {
        TModel ExportModel(string widgetId);
    }
}
