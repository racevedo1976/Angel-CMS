using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Documents.Data;
using Angelo.Connect.Documents.Models;
using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Data;

namespace Angelo.Connect.Documents.Services
{
    public class DocumentListService
    {
        private DocumentListDbContext _db;
        private ConnectDbContext _connectDb;

        public DocumentListService(DocumentListDbContext db, ConnectDbContext connectDb)
        {
            _db = db;
            _connectDb = connectDb;
        }

        public async Task<IList<DocumentListDocument>> GetItems(string widgetId)
        {
            return await _db.DocumentListDocuments.Where(x => x.WidgetId == widgetId).ToListAsync();
        }

        public async Task<IList<DocumentListFolder>> GetFolders(string widgetId)
        {
            return await _db.DocumentListFolders.Where(x => x.WidgetId == widgetId).ToListAsync();
        }

        public async Task<bool> AddItem(DocumentListDocument item)
        {
            var sort = 0;
            var createdDocuments = _db.DocumentListDocuments.Where(x => x.FolderId == null).OrderBy(y => y.Sort).ToList();

            item.Id = KeyGen.NewGuid();
            item.Sort = sort;

            foreach (var document in createdDocuments)
            {
                document.Sort = sort++;
            }

            _db.DocumentListDocuments.Add(item);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddNewFolder(string widgetId)
        {
            var folder = new DocumentListFolder()
            {
                Id = KeyGen.NewGuid(),
                WidgetId = widgetId,
                Title = "New Folder",
                Sort = 0
                
            };
           

            _db.DocumentListFolders.Add(folder);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveTitle(string id, string title)
        {
            var widget = await  _db.DocumentListWidgets.FirstOrDefaultAsync(x => x.Id == id);

            widget.Title = title;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateSortOrder(string id, int sortNumber, string parentFolderId)
        {
            var document = await _db.DocumentListDocuments.FirstOrDefaultAsync(x => x.Id == id);

            document.Sort = sortNumber;
            document.FolderId = parentFolderId;

            await _db.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdateFolderSortOrder(string id, int sortNumber)
        {
            var folder = await _db.DocumentListFolders.FirstOrDefaultAsync(x => x.Id == id);

            folder.Sort = sortNumber;
            
            await _db.SaveChangesAsync();

            return true;
        }


        
        public async Task<bool> DeleteItem(string id)
        {
            var item = await _db.DocumentListDocuments.FirstOrDefaultAsync(x => x.Id == id);

            _db.DocumentListDocuments.Remove(item);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteFolder(string id)
        {
            var item = await _db.DocumentListFolders.Include(x => x.Documents).FirstOrDefaultAsync(x => x.Id == id);

            foreach (var doc in item.Documents)
            {
                _db.DocumentListDocuments.Remove(doc);
            }

            _db.DocumentListFolders.Remove(item);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveDocumentTitle(string id, string title)
        {
            var document = await _db.DocumentListDocuments.FirstOrDefaultAsync(x => x.Id == id);

            document.Title = title;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveDocumentUrl(string id, string url)
        {
            var document = await _db.DocumentListDocuments.FirstOrDefaultAsync(x => x.Id == id);

            document.Url = url;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveFolderTitle(string id, string title)
        {
            var folder = await _db.DocumentListFolders.FirstOrDefaultAsync(x => x.Id == id);

            folder.Title = title;

            await _db.SaveChangesAsync();

            return true;
        }
    }
}
