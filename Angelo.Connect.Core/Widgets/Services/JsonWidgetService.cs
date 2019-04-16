using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Angelo.Connect.Widgets.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Widgets;
using AutoMapper.Extensions;

namespace Angelo.Connect.Widgets.Services
{
    public abstract class JsonWidgetService<TModel> : IWidgetService<TModel>, IWidgetExportService<TModel>
    where TModel : class, IWidgetModel
    {
        private ConnectDbContext _db;

        public JsonWidgetService(ConnectDbContext db)
        {
            _db = db;
        }

        public void SaveModel(TModel model)
        {
            model.Id = Guid.NewGuid().ToString("N");

            _db.Add(new JsonWidget
            {
                Id = model.Id,
                ModelType = model.GetType().FullName,
                ModelJson = JsonConvert.SerializeObject(model)
            });

            _db.SaveChanges();
        }

        public TModel CloneModel(TModel model)
        {
            var cloned = model.Clone();

            cloned.Id = Guid.NewGuid().ToString("N");

            _db.JsonWidgets.Add(new JsonWidget
            {
                Id = model.Id,
                ModelType = model.GetType().FullName,
                ModelJson = JsonConvert.SerializeObject(model)
            });
            _db.SaveChanges();

            return cloned;
        }

        public void UpdateModel(TModel model)
        {
            var widget = new JsonWidget
            {
                Id = model.Id,
                ModelType = model.GetType().FullName,
                ModelJson = JsonConvert.SerializeObject(model)
            };

            _db.Attach<JsonWidget>(widget);
            _db.Entry(widget).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _db.SaveChanges();
        }

        public void DeleteModel(string widgetId)
        {
            var widget = _db.JsonWidgets.FirstOrDefault(x => x.Id == widgetId);

            _db.JsonWidgets.Remove(widget);
            _db.SaveChanges();
        }

        public TModel GetModel(string widgetId)
        {
            var widget = _db.JsonWidgets.AsNoTracking().FirstOrDefault(x => x.Id == widgetId);

            if (widget != null && !String.IsNullOrEmpty(widget.ModelJson))
            {
                return JsonConvert.DeserializeObject<TModel>(widget.ModelJson);
            }

            return default(TModel);
        }

        public abstract TModel GetDefaultModel();

        /// <summary>
        /// This should be overridden in derived services if the service model contains
        /// FKs to related data in order to prevent accidential cross-site data leaks upon import
        /// </summary>
        public virtual TModel ExportModel(string widgetId)
        {
            // Core Widget's don't currently use complex models with FK's to other data
            // which is why this base implementation is provided

            var model = GetModel(widgetId);

            model.Id = null;

            return model;
        }

    }
}
