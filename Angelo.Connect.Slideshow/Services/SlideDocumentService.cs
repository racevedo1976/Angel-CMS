using Angelo.Connect.Abstractions;
using Angelo.Connect.SlideShow.Data;
using Angelo.Connect.SlideShow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Models;

namespace Angelo.Connect.SlideShow.Services
{
    public class SlideDocumentService : IDocumentService<Slide>
    {
        #region Dependencies
        private SlideShowDbContext _readDb;
        private DbContextOptions<SlideShowDbContext> _writeDb;
        #endregion // Dependencies
        #region Constructors
        public SlideDocumentService(SlideShowDbContext readDb, DbContextOptions<SlideShowDbContext> writeDb)
        {
            Ensure.NotNull(readDb);
            Ensure.NotNull(writeDb);

            this._readDb = readDb;
            this._writeDb = writeDb;
        }
        #endregion // Constructors
        #region IDocumentService<Slide> implementation
        public async Task<Slide> CloneAsync(Slide document)
        {
            Ensure.NotNull(document, $"{nameof(document)} cannot be null.");

            var result = new Slide()
            {
                DocumentId = KeyGen.NewGuid(),
                BackgroundFit = document.BackgroundFit,
                Color = document.Color,
                Delay = document.Delay,
                Direction = document.Direction,
                Duration = document.Duration,
                ImageSourceSize = document.ImageSourceSize,
                ImageUrl = document.ImageUrl,
                IsLinkEnabled = document.IsLinkEnabled,
                KenBurnsEffect = document.KenBurnsEffect,
                Layers = document.Layers,
                LinkTarget = document.LinkTarget,
                Parallax = document.Parallax,
                Position = document.Position,
                SlideLinkUrl = document.SlideLinkUrl,
                SlotBoxAmount = document.SlotBoxAmount,
                SlotRotation = document.SlotRotation,
                State = document.State,
                ThumbnailUrl = document.ThumbnailUrl,
                Tiling = document.Tiling,
                Title = document.Title,
                Transition = document.Transition,
                VisibleFrom = document.VisibleFrom,
                VisibleTo = document.VisibleTo
            };

            // Copy the DB entry
            result = await this.CreateAsync(result);

            return result;
        }

        public async Task<Slide> CreateAsync(Slide document)
        {
            using (var db = new SlideShowDbContext(_writeDb))
            {
                db.Slides.Add(document);
                await db.SaveChangesAsync();
            }

            return document;
        }

        public async Task DeleteAsync(Slide document)
        {
            Ensure.NotNull(document);

            await DeleteAsync(document.DocumentId);
        }

        public async Task DeleteAsync(string documentId)
        {
            using (var db = new SlideShowDbContext(_writeDb))
            {
                var document = await db.Slides.FirstOrDefaultAsync(x => x.DocumentId == documentId);

                db.Slides.Remove(document);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteByIdsAsync(IEnumerable<string> documentIds)
        {
            using (var db = new SlideShowDbContext(_writeDb))
            {
                var delete = documentIds.Select(x => db.Slides.SingleOrDefault(y => y.DocumentId == x)).Where(x => x != null);

                foreach (var item in delete)
                {
                    db.Slides.Remove(item);
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task<Slide> GetAsync(Slide document)
        {
            Ensure.NotNull(document);
            return await GetAsync(document.DocumentId);
        }

        public async Task<Slide> GetAsync(string documentId)
        {
            Ensure.NotNullOrEmpty(documentId);

            using (var db = new SlideShowDbContext(_writeDb))
            {
                return await db.Slides.SingleOrDefaultAsync(x => x.DocumentId == documentId);
            }

                
        }

        public Task<DocumentLibrary> GetDocumentLibraryAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDocumentLibraryLocation(string documentId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Slide> Query()
        {
            return _readDb.Slides.Include(x => x.Layers);
        }

        public IQueryable<Slide> QueryByIds(IEnumerable<string> documentIds)
        {
            return _readDb.Slides
                .Where(x => documentIds.Contains(x.DocumentId))
                ;
        }

        public async Task<Slide> RenameAsync(Slide document)
        {
            return await UpdateAsync(document);
        }

        public async Task<Slide> UpdateAsync(Slide document)
        {
            // TODO: How to handle non-Drive image URLs?  Thumbnails handled how?
            using (var db = new SlideShowDbContext(_writeDb))
            {
                db.Attach(document);
                db.Entry(document).State = EntityState.Modified;

                await db.SaveChangesAsync();
            }

            return document;
        }

        
        #endregion // IDocumentService<Slide> implementation
    }
}
