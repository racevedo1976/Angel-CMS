using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Plugins;

namespace Angelo.Connect.Widgets
{
    public class WidgetConfig : IPluginData
    {
        public string Id { get; set; }

        public string WidgetType { get; set; }
        public string WidgetName { get; set; }
        public string Category { get; set; }
        public string IconUrl { get; set; }

        public bool ShowInToolbar { get; set; } = true;

        public IEnumerable<WidgetViewEntry> Views { get { return ViewList; } }
        public IEnumerable<WidgetFormEntry> Forms { get { return FormList; } }
        public WidgetEditorEntry Editor { get { return InlineEditor; } }

        public bool HasEditor { get { return InlineEditor?.ComponentType != null; } }
        public bool HasSettings { get { return FormList.Count > 0; } }

        internal Type ServiceType;
        internal Type ModelType;

        internal Func<IServiceProvider, object> GetDefaultSettings;
        internal Func<IServiceProvider, string, IWidgetModel> GetSettings;
        internal Func<IServiceProvider, string, IWidgetModel> CloneSettings;
        internal Func<IServiceProvider, string, IWidgetModel> ExportSettings;
        internal Func<IServiceProvider, string, ViewComponent> GetSettingsForm;
        internal Action<IServiceProvider, object> SaveSettings;

        internal WidgetEditorEntry InlineEditor;
        internal List<WidgetViewEntry> ViewList;
        internal List<WidgetFormEntry> FormList;
        internal List<Func<WidgetContext, IMenuItem>> MenuItems;

        public WidgetConfig()
        {
            ViewList = new List<WidgetViewEntry>();
            FormList = new List<WidgetFormEntry>();
            MenuItems = new List<Func<WidgetContext, IMenuItem>>();
        }

        public string GetDefaultViewId()
        {
            if (ViewList.Count == 0)
                throw new Exception($"No views registered for widget {WidgetType}");

            return ViewList[0].Id;
        }

        public string GetViewPath(string viewId = null)
        {
            if (viewId == null)
                viewId = GetDefaultViewId();

            var view = ViewList.FirstOrDefault(x => x.Id == viewId);

            if (view == null)
                throw new Exception($"Could not locate view {viewId} for widget {WidgetType}");

            return view.Path;
        }
    }
}
