using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Documents.Models;
using Angelo.Connect.Documents.Data;
using Angelo.Connect.Configuration;
using Angelo.Connect.Widgets;
using AutoMapper.Extensions;

namespace Angelo.Connect.Documents.Services
{
    public class DocumentListWidgetService : IWidgetService<DocumentListWidget>
    {
        private DbContextOptions<DocumentListDbContext> _db;
        private SiteContext _siteContext;

        public DocumentListWidgetService(DbContextOptions<DocumentListDbContext> db,
            SiteContext siteContext)
        {
            _db = db;
            _siteContext = siteContext;
        }

        public void DeleteModel(string widgetId)
        {
            using (var db = new DocumentListDbContext(_db))
            {
                var model = db.DocumentListWidgets.Include(x => x.Documents).FirstOrDefault(x => x.Id == widgetId);
                if (model == null) return;      // Another view/session already deleted it (race condition)

                db.DocumentListWidgets.Remove(model);
                db.SaveChanges();
            }
        }

        public DocumentListWidget GetDefaultModel()
        {
            return new DocumentListWidget
            {
                Id = Guid.NewGuid().ToString(),
                SiteId = _siteContext.SiteId
            };
        }

        public DocumentListWidget GetModel(string widgetId)
        {
            using (var db = new DocumentListDbContext(_db))
            {
                return db.DocumentListWidgets
                    .Include(x => x.Documents)
                    .Include(x => x.Folders)
                    .FirstOrDefault(x => x.Id == widgetId);
            }
        }

        public void SaveModel(DocumentListWidget model)
        {
            using (var db = new DocumentListDbContext(_db))
            {
                db.DocumentListWidgets.Add(model);
                db.SaveChanges();
            }
        }

        public void UpdateModel(DocumentListWidget model)
        {
            using (var db = new DocumentListDbContext(_db))
            {
                db.Attach<DocumentListWidget>(model);
                db.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                db.SaveChanges();
            }
        }

        public DocumentListWidget CloneModel(DocumentListWidget model)
        {
            var originalWidgetId = model.Id;
            var cloned = new DocumentListWidget()
            {
                Id = Guid.NewGuid().ToString("N"),
                SiteId = model.SiteId,
                Title = model.Title,
                Documents = new List<DocumentListDocument>(),
                Folders = new List<DocumentListFolder>(),
            };

            cloned.Documents = model.Documents.Select(x =>
                new DocumentListDocument()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Sort = x.Sort,
                    Title = x.Title,
                    WidgetId = cloned.Id,
                    ThumbnailUrl = x.ThumbnailUrl,
                    DocumentId = x.DocumentId,
                    FolderId = x.FolderId,
                    Url = x.Url
                }).ToList();

            var clonedFolders = new List<DocumentListFolder>();
            foreach (var folder in model.Folders.ToList())
            {
                var originalId = folder.Id;
                var clonedFolderId = Guid.NewGuid().ToString("N");
                clonedFolders.Add(new DocumentListFolder()
                {
                    WidgetId = cloned.Id,
                    Sort = folder.Sort,
                    Title = folder.Title,
                    Id = clonedFolderId
                });

                foreach (var clonedDocs in cloned.Documents.Where(x => x.FolderId == originalId))
                {
                    clonedDocs.FolderId = clonedFolderId;
                }
            }

            cloned.Folders = clonedFolders;
            
            using (var db = new DocumentListDbContext(_db))
            {
               
                db.DocumentListWidgets.Add(cloned);
                db.SaveChanges();
            }

            return cloned;
        }
    }
}
