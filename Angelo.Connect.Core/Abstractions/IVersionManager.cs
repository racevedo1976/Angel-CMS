using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IVersionManager
    {
        bool AllowMultipleDrafts { get; }

        Task<IEnumerable<IContentVersion>> GetVersions(string contentId);

        Task<IContentVersion> GetPublishedVersion(string contentId);

        /// <summary>
        /// Used to render page when no published version is available, such as when page is first created
        /// </summary>
        Task<IContentVersion> GetLatestVersion(string contentId);

        Task<IContentVersion> CreateDraftVersion(string contentId, string versionLabel, string fromVersionCode = null);

        Task UpdateVersionLabel(string contentId, string versionCode, string versionLabel);

        Task PublishVersion(string pageId, string versionCode);

        Task DeleteVersion(string pageId, string versionCode);
    }
}
