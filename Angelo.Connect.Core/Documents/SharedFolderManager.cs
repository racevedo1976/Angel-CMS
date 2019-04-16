using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Common.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Extensions;
using Angelo.Connect.Security;

namespace Angelo.Connect.Services
{
    public class SharedFolderManager
    {
        private ConnectDbContext _db;

        public SharedFolderManager(ConnectDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<Folder> GetFolderAsync(string folderId)
        {
            var folder = await _db.Folders.AsNoTracking()
                .Include(x => x.Security)
                .Include(x => x.Sharing)
                .Include(x => x.Tags)
                .Where(x => x.Id == folderId)
                .FirstOrDefaultAsync();
            return folder;
        }

        public async Task<ICollection<DocumentType>> GetDocumentTypesAsync()
        {
            var list =  new List<DocumentType>()
            {
                new DocumentType() {
                    Id = string.Empty,
                    Title = "(Undefined Type)"
                },
                new DocumentType() {
                    Id = "Canvas.Plugins.Models.File",
                    Title = "File"
                },
                new DocumentType() {
                    Id = "Canvas.Plugins.Models.BlogPost",
                    Title = "Blog Post"
                },
                new DocumentType() {
                    Id = "Canvas.Plugins.Models.Image",
                    Title = "Image"
                },
                new DocumentType() {
                    Id = "Canvas.Plugins.Models.Video",
                    Title = "Video"
                },
                new DocumentType() {
                    Id = "Canvas.Plugins.Models.CalendarEvent",
                    Title = "Calender Event"
                },
                new DocumentType() {
                    Id = "Canvas.Plugins.Models.HomeworkAssignment",
                    Title = "Homework Assignment"
                }
            };
            return await Task.FromResult(list);
        }

        public async Task<ICollection<Folder>> GetClientFoldersAsync(string clientId)
        {
            var poolId = await _db.Clients.AsNoTracking()
                .Where(x => x.Id == clientId)
                .Select(s => s.SecurityPoolId)
                .FirstOrDefaultAsync();

            var folders = await _db.Folders.AsNoTracking()
                .Where(x => x.OwnerId == poolId)
                .ToListAsync();

            return folders;
        }

        public async Task<ICollection<Folder>> GetSiteFoldersAsync(string siteId)
        {
            var poolId = await _db.Sites.AsNoTracking()
                .Where(x => x.Id == siteId)
                .Select(s => s.SecurityPoolId)
                .FirstOrDefaultAsync();

            var folders = await _db.Folders.AsNoTracking()
                .Where(x => x.OwnerId == poolId)
                .ToListAsync();

            return folders;
        }

        public async Task<bool> UpdateFolderAsync(Folder folder)
        {
            var folder1 = await _db.Folders.Where(x => x.Id == folder.Id).FirstOrDefaultAsync();
            if (folder1 == null)
                return false;
            else
            {
                folder1.Title = folder.Title;
                folder1.DocumentType = folder.DocumentType;
                await _db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Folder> InsertClientFolderAsync(string clientId, string title, string documentType, string parentId = null)
        {
            var poolId = await _db.Clients.AsNoTracking()
                .Where(x => x.Id == clientId)
                .Select(s => s.SecurityPoolId)
                .FirstOrDefaultAsync();

            var folder = new Folder();
            folder.Id = Guid.NewGuid().ToString();
            folder.Title = title;
            folder.DocumentType = documentType;
            folder.ParentId = parentId;
            folder.OwnerId = poolId;
            folder.OwnerLevel = OwnerLevel.Client;
            folder.FolderFlags = FolderFlag.Shared;
            _db.Folders.Add(folder);
            await _db.SaveChangesAsync();
            return folder;
        }

        public async Task<Folder> InsertSiteFolderAsync(string siteId, string title, string documentType, string parentId = null)
        {
            var poolId = await _db.Sites.AsNoTracking()
                .Where(x => x.Id == siteId)
                .Select(s => s.SecurityPoolId)
                .FirstOrDefaultAsync();

            var folder = new Folder();
            folder.Id = Guid.NewGuid().ToString();
            folder.Title = title;
            folder.DocumentType = documentType;
            folder.ParentId = parentId;
            folder.OwnerId = poolId;
            folder.OwnerLevel = OwnerLevel.Site;
            folder.FolderFlags = FolderFlag.Shared;
            _db.Folders.Add(folder);
            await _db.SaveChangesAsync();
            return folder;
        }

        public async Task DeleteFolder(string id)
        {
            var folder = await _db.Folders.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (folder != null)
            {
                // TO DO: delete child folders and any many-to-many relations.
                _db.Folders.Remove(folder);
                await _db.SaveChangesAsync();
            }
        }


    }
}
