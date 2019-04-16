using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public class TagManager
    {
        private ConnectDbContext _db;

        public TagManager(ConnectDbContext dbContext)
        {
            _db = dbContext;
        }

        #region General
        public async Task<IEnumerable<Tag>> GetUserTags(string userId)
        {
            return await _db.Tags
                .Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Tag>> GetAllTags()
        {
            return await _db.Tags
                        .OrderBy(x => x.TagName)
                        .ToListAsync();
        }

        public async Task<Tag> GetById(string id)
        {
            return await _db.Tags
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Tag> GetByName(string userId, string tagName)
        {
            return await _db.Tags.SingleOrDefaultAsync(x => x.UserId == userId && x.TagName == tagName);
        }

        public async Task<Tag> AddTag(string userId, string tagName)
        {
            var existingTag = await _db.Tags.FirstOrDefaultAsync(x => x.UserId == userId && x.TagName == tagName);

            if (existingTag != null)
                return existingTag;

            var newTag = new Tag
            {
                Id = Guid.NewGuid().ToString("N"),
                TagName = tagName,
                UserId = userId,
                IsActive = true,
            };

            _db.Tags.Add(newTag);

            await _db.SaveChangesAsync();

            return newTag;
        }

        public async Task AddTag(Tag tag)
        {
            var existingTag = await _db.Tags.FirstOrDefaultAsync(x => x.UserId == tag.UserId && x.TagName == tag.TagName);

            if (existingTag != null)
            {
                tag.Id = existingTag.Id;
                tag.IsActive = existingTag.IsActive;
            }
            else
            {
                var newTag = new Tag
                {
                    Id = Guid.NewGuid().ToString("N"),
                    TagName = tag.TagName,
                    UserId = tag.UserId,
                    IsActive = tag.IsActive,
                };

                _db.Tags.Add(newTag);

                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteTag(string tagId)
        {
            if (string.IsNullOrEmpty(tagId))
                throw new ArgumentNullException("tagId");

            var tag = await GetById(tagId);

            if(tag != null)
            {
                _db.Remove(tag);
                await _db.SaveChangesAsync();
            }

        }

        public async Task DeleteTag(string userId, string tagName)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("userId");

            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentNullException("tagName");

            var tag = await GetByName(userId, tagName);

            if (tag != null)
            {
                _db.Remove(tag);
                await _db.SaveChangesAsync();
            }
        }

        public async void UpdateTag(Tag tag)
        {
            _db.Update(tag);
            await _db.SaveChangesAsync();
        }
        #endregion // General

        #region Folders
        public async Task AddTag(IFolder folder, Tag tag)
        {
            Ensure.NotNull(folder, $"{nameof(folder)} cannot be null.");
            Ensure.NotNull(tag, $"{nameof(tag)} cannot be null.");

            var folderTag = _db.FolderTags.FirstOrDefault(x => x.TagName == tag.TagName && x.FolderId == folder.Id);

            //If already added, return
            if (folderTag != null)
            {
                return;
            }

            folderTag = new FolderTag
            {
                FolderId = folder.Id,
                TagName = tag.TagName
            };

            _db.FolderTags.Add(folderTag);
            await _db.SaveChangesAsync();

        }

        public async Task RemoveTag(IFolder folder, Tag tag)
        {
            Ensure.NotNull(folder, $"{nameof(folder)} cannot be null.");
            Ensure.NotNull(tag, $"{nameof(tag)} cannot be null.");


            var folderTag = _db.FolderTags.FirstOrDefault(x => x.TagName == tag.TagName && x.FolderId == folder.Id);

            if (folderTag == null)
            {
                return;
            }

            _db.FolderTags.Remove(folderTag);
            await _db.SaveChangesAsync();
        }
        #endregion // Folders

        #region Documents
        public async Task AddTag(IDocument document, Tag tag)
        {
            Ensure.NotNull(document, $"{nameof(document)} cannot be null.");
            Ensure.NotNull(tag, $"{nameof(tag)} cannot be null.");

            var docTag = _db.DocumentTags.FirstOrDefault(x => x.TagName == tag.TagName && x.DocumentId == document.DocumentId);

            //If already added, return
            if (docTag != null)
            {
                return;
            }

            docTag = new DocumentTag
            {
                DocumentId = document.DocumentId,
                TagName = tag.TagName
            };

            _db.DocumentTags.Add(docTag);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveTag(IDocument document, Tag tag)
        {
            Ensure.NotNull(document, $"{nameof(document)} cannot be null.");
            Ensure.NotNull(tag, $"{nameof(tag)} cannot be null.");

            var docTag = _db.DocumentTags.FirstOrDefault(x => x.TagName == tag.TagName && x.DocumentId == document.DocumentId);

            if (docTag == null)
            {
                return;
            }

            _db.DocumentTags.Remove(docTag);
            await _db.SaveChangesAsync();
        }
        #endregion // Documents
    }
}
