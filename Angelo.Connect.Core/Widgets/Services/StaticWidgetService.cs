using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets.Models;

namespace Angelo.Connect.Widgets.Services
{
    public class StaticWidgetService : IWidgetService<StaticWidget>
    {
        public StaticWidget CloneModel(StaticWidget model)
        {
            // return new empty instance with different id
            return new StaticWidget { Id = Guid.NewGuid().ToString() };
        }

        public void DeleteModel(string widgetId)
        {
            // nothing is being persisted so nothing to delete
        }

        public StaticWidget GetDefaultModel()
        {
            return new StaticWidget();
        }

        public StaticWidget GetModel(string widgetId)
        {
            // return empty instance with requested id
            return new StaticWidget { Id = widgetId };
        }

        public void SaveModel(StaticWidget model)
        {
            // static widgets don't require persistance
        }

        public void UpdateModel(StaticWidget model)
        {
            // nothing is persisted, so nothing to update
        }
    }
}
