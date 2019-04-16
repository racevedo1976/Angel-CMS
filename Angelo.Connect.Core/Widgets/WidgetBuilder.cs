using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Menus;

namespace Angelo.Connect.Widgets
{
    public class WidgetBuilder<TWidgetModel> where TWidgetModel : class, IWidgetModel
    {

        private WidgetConfig _widget;

        public WidgetBuilder(WidgetConfig config)
        {
            _widget = config;
        }

        public WidgetBuilder<TWidgetModel> AddView(Action<WidgetViewEntry> viewBuilder)
        {
            var view = new WidgetViewEntry();

            viewBuilder.Invoke(view);

            if (string.IsNullOrEmpty(view.Id))
                view.Id = "default";

            if (_widget.ViewList.Any(x => x.Id == view.Id))
                throw new Exception($"Cannot register widget view. View Id {view.Id} already exists.");

            if (string.IsNullOrEmpty(view.Path))
                throw new Exception($"Cannot register widget. View Path is required.");

            if (string.IsNullOrEmpty(view.Title))
                throw new Exception($"Cannot register widget. View Title is required.");
           
            _widget.ViewList.Add(view);

            return this;
        }

        public WidgetBuilder<TWidgetModel> AddEditor<TEditorComponent>() where TEditorComponent : ViewComponent, IWidgetComponent<TWidgetModel>
        {
            if (_widget.HasEditor)
                throw new Exception("An inline editor has already been registered for this widget.");

            _widget.InlineEditor = new WidgetEditorEntry(_widget, typeof(TEditorComponent));

            return this;
        }

        public WidgetBuilder<TWidgetModel> AddForm<TFormComponent>(Action<WidgetFormEntry> formBuilder) where TFormComponent : ViewComponent
        {
            var form = new WidgetFormEntry(_widget, typeof(TFormComponent));

            formBuilder.Invoke(form);

            _widget.FormList.Add(form);

            return this;
        }

        public WidgetBuilder<TWidgetModel> AddMenuOption(Func<WidgetContext<TWidgetModel>, IMenuItem> menuItemBuilder)
        {
            _widget.MenuItems.Add((widgetContext) => {

                var typedContext = new WidgetContext<TWidgetModel>(widgetContext);

                return menuItemBuilder.Invoke(typedContext);
            });

            return this;
        }

    }
}
