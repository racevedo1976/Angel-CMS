using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;

namespace Angelo.Connect.Services
{
    public class FileDocumentService : IDocumentService<FileDocument>
    {

        #region Dependencies
        private ConnectDbContext _readDb;
        private DbContextOptions<ConnectDbContext> _writeDb;
        #endregion // Dependencies
        #region Constructors
        public FileDocumentService(ConnectDbContext readDb, DbContextOptions<ConnectDbContext> writeDb)
        {
            Ensure.NotNull(readDb);
            Ensure.NotNull(writeDb);

            this._readDb = readDb;
            this._writeDb = writeDb;
        }
        #endregion // Constructors
        #region IDocumentService<Document> implementation
        public async Task<FileDocument> CloneAsync(FileDocument document)
        {
            Ensure.NotNull(document, $"{nameof(document)} cannot be null.");

            var result = new FileDocument()
            {
                ContentLength = document.ContentLength,
                Created = document.Created,
                Description = document.Description,
                FileName = document.FileName,
                FileType = document.FileType,
                DocumentId = KeyGen.NewGuid()
            };

            // Copy the DB entry
            result = await this.CreateAsync(result);

            return result;
        }

        public virtual async Task<FileDocument> CreateAsync(FileDocument document)
        {
            //  await EnsureDocumentQuotaAsync(document);

            // Create database entry
            using (var db = new ConnectDbContext(_writeDb))
            {
                db.FileDocuments.Add(document);
                db.SaveChanges();
            }

            return document;
        }

        public virtual async Task DeleteAsync(string documentId)
        {
            var documentItems = _readDb
                .FolderItems
                .Where(x => x.DocumentId == documentId).ToList();

            if (documentItems != null)
            {
                _readDb.FolderItems.RemoveRange(documentItems);
                await _readDb.SaveChangesAsync();
            }

            var document = _readDb
               .FileDocuments
               .FirstOrDefault(x => x.DocumentId == documentId);

            if (document != null)
            {
                _readDb.FileDocuments.Remove(document);
                await _readDb.SaveChangesAsync();
            }

            //TODO Send request to Drive to remove physical file
        }

        public async Task DeleteAsync(FileDocument document)
        {
            Ensure.NotNull(document);

            await DeleteAsync(document.DocumentId);
        }

        public virtual async Task DeleteByIdsAsync(IEnumerable<string> documentIds)
        {
            var delete = documentIds.Select(x => _readDb.FileDocuments.SingleOrDefault(y => y.DocumentId == x)).Where(x => x != null);

            foreach (var item in delete)
            {
                await DeleteAsync(item);

            }
        }

        public async Task<FileDocument> GetAsync(FileDocument document)
        {
            Ensure.NotNull(document);

            return await GetAsync(document.DocumentId);
        }

        public async Task<FileDocument> GetAsync(string documentId)
        {
            Ensure.NotNullOrEmpty(documentId);

            return await _readDb.FileDocuments.SingleOrDefaultAsync(x => x.DocumentId == documentId);
        }

        public async Task<DocumentLibrary> GetDocumentLibraryAsync(string id)
        {
            var location = string.Empty;

            //attempt to get the library location
            var lib = await _readDb.FolderItems
                        .Include(x => x.Folder)
                        .ThenInclude(x => x.DocumentLibrary)
                        .Where(x => x.DocumentId == id)
                        .FirstOrDefaultAsync();

            
            return lib.Folder.DocumentLibrary;
        }

        public async Task<string> GetDocumentLibraryLocation(string documentId)
        {
            var location = string.Empty;

            //attempt to get the library location
            var lib = await _readDb.FolderItems
                        .Include(x => x.Folder)
                        .ThenInclude(x => x.DocumentLibrary)
                        .Where(x => x.DocumentId == documentId)
                        .FirstOrDefaultAsync();

            if (lib != null)
                location = lib.Folder.DocumentLibrary.Location;

            return location;
        }

        public IQueryable<FileDocument> Query()
        {
            return _readDb.FileDocuments;
        }

        public IQueryable<FileDocument> QueryByIds(IEnumerable<string> documentIds)
        {
            return _readDb.FileDocuments
                .Where(x => documentIds.Contains(x.DocumentId))
                ;
        }

        public async Task<FileDocument> RenameAsync(FileDocument document)
        {
            return await UpdateAsync(document);
        }

        public async Task<FileDocument> UpdateAsync(FileDocument document)
        {
            using (var db = new ConnectDbContext(_writeDb))
            {
                db.Attach(document);
                db.Entry(document).State = EntityState.Modified;

                db.SaveChanges();
            }

            return document;
        }
        //private async Task EnsureDocumentQuotaAsync(string ownerId)
        //{
        //    var quota = this.DocumentQuota;
        //    var count = await GetTotalDocumentCountAsync(ownerId);

        //    if (count >= quota)
        //    {
        //        throw new InvalidOperationException($"User is limited to {quota} documents.");
        //    }
        //}

        //private async Task<int> GetTotalDocumentCountAsync(string ownerId)
        //{
        //    return await _db.FileDocuments
        //        //.Where(x => x.Folder.OwnerUserId == ownerId)
        //        .CountAsync();
        //}
        #endregion // Quota workers
    }
}
