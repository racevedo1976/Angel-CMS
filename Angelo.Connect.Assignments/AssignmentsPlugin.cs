
using Microsoft.Extensions.DependencyInjection;

using Angelo.Connect.Icons;
using Angelo.Connect.Assignments.Data;
using Angelo.Connect.Assignments.Services;
using Angelo.Connect.Assignments.UI.Components;
using Angelo.Connect.Assignments.ViewModels;
using Angelo.Connect.Widgets;
using Angelo.Plugins;


namespace Angelo.Connect.Assignments
{
    public class AssignmentsPlugin: IPlugin
    {
        public string Name { get; } = "Assignments Plugin";
        public string Version { get; } = "0.0.1";
        public string Description { get; } = "Plugin created during demo";
        public string Author { get; } = "SchoolInSites";

        public void Startup(PluginBuilder pluginBuilder)
        {

            pluginBuilder.ConfigureServices(services =>
            {
                //services.Configure<ClientMenu>(menu =>
                //{
                //    menu.MenuItems.Add(new MenuItemStatic
                //    {
                //        Icon = IconType.Briefcase,
                //        Title = "Live Streams",
                //        Url = "/Admin/VideoStreamLink",
                //    });
                //});
                services.AddTransient<AssignmentManager>();
            });

            pluginBuilder.AddDbContext<AssignmentsDbContext>(db => {
                AssignmentsDbActions.CreateSchemas(db);
                AssignmentsDbActions.CreateTables(db);
                AssignmentsDbActions.InsertSeedData(db);
                //AssignmentsAutomapper.ConfigureAutomapper();
            });

            pluginBuilder.RegisterWidget<AssignmentsWidgetService, AssignmentsWidgetViewModel>(widget =>
            {
                widget.Category = "Apps";
                widget.WidgetType = "assignments";
                widget.WidgetName = "Assignments List";
            })
            .AddForm<AssignmentsWidgetConfigForm>(f =>
            {
                f.Title = "Settings";
                f.AjaxFlags = AjaxFlags.ALL;
            })
            .AddView(view =>
            {
                view.Id = "assignments-list";
                view.Title = "Assignments List";
                view.Path = "~/UI/Views/Widgets/AssignmentsList.cshtml";
                //view.IconImage = "Assignments.svg";
                view.IconClass = "icon-Assignments";
            });

        }
    }
}
