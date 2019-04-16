using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.Widgets
{
    public interface IWidgetNamedModelProvider
    {
        IWidgetModel GetModel(string widgetType, string modelName, Type modelType);
    }
}
