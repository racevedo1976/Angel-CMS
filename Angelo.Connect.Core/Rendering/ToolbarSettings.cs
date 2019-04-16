using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.UI.Components;

namespace Angelo.Connect.Rendering
{
    public class ToolbarSettings
    {
        internal Type ComponentType { get; private set; }
        internal object ComponentArguments { get; private set; }

        internal ToolbarSettings(Type componentType, object arguments)
        {
            ComponentType = componentType;
            ComponentArguments = arguments;
        }

        public ToolbarSettings(string toolbarView, object toolbarModel)
        {
            ComponentType = typeof(GenericView);
            ComponentArguments = new {
                view = toolbarView,
                model = toolbarModel
            };
        }

        public ToolbarSettings(string toolbarView)
        {
            ComponentType = typeof(GenericView);
            ComponentArguments = new { };
        }

        public ToolbarSettings() { }
    }


    public class ToolbarSettings<TViewComponent> : ToolbarSettings where TViewComponent : ViewComponent
    {
        public ToolbarSettings(object parameters) : base(typeof(TViewComponent), parameters)
        {

        }

        public ToolbarSettings() : base (typeof(TViewComponent), new { })
        {

        }
    }
}
