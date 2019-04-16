using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Configuration;
using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class SharedContentData1Controller : BaseController
    {
        const string TYPE_NODE_ID_PREFIX = "TYPE:";
        const string FOLDER_NODE_ID_PREFIX = "FOLDER:";

        private SharedFolderManager _folderManager;
        private PageManager _pageManager;
        private SiteManager _siteManager;
        private ClientManager _clientManager;
        private IOptions<RequestLocalizationOptions> _localizationOptions;

        public SharedContentData1Controller(SharedFolderManager folderManager, PageManager pageManager, SiteContext siteContext,
            SiteManager siteManager, ILogger<SecurityPoolManager> logger,
            ClientManager clientManager, IOptions<RequestLocalizationOptions> localizationOptions) : base(logger)
        {
            _folderManager = folderManager;
            _pageManager = pageManager;
            _siteManager = siteManager;
            _clientManager = clientManager;
            _localizationOptions = localizationOptions;
        }
      
        protected List<SharedFolderTreeNodeViewModel> CreateSharedFolderViewModelList(ICollection<Folder> folders, ICollection<DocumentType> documentTypes)
        {
           var list = new List<SharedFolderTreeNodeViewModel>();

            // Add the root document type nodes to the tree list.
            foreach (var docType in documentTypes)
            {
                list.Add(new SharedFolderTreeNodeViewModel()
                {
                    NodeId = TYPE_NODE_ID_PREFIX + docType.Id,
                    ParentNodeId = string.Empty,
                    DocumentType = docType.Id,
                    FolderId = string.Empty,
                    NodeType = SharedFolderTreeNodeType.TypeNode,
                    Title = docType.Title
                });
            }

            // Add the folders nodes to the tree list
            foreach (var folder in folders)
            {
                string nodeParentId;
                if (string.IsNullOrEmpty(folder.ParentId))
                    nodeParentId = TYPE_NODE_ID_PREFIX + (folder.DocumentType ?? string.Empty);
                else
                    nodeParentId = FOLDER_NODE_ID_PREFIX + folder.ParentId;
                list.Add(new SharedFolderTreeNodeViewModel()
                {
                    NodeId = FOLDER_NODE_ID_PREFIX + folder.Id,
                    ParentNodeId = nodeParentId,
                    DocumentType = folder.DocumentType,
                    FolderId = folder.Id,
                    NodeType = SharedFolderTreeNodeType.FolderNode,
                    Title = folder.Title
                });
            }

            return list;
        }

        [Authorize]
        [HttpPost, Route("/api/clients/shared-folders")]
        public async Task<JsonResult> GetClientFolders([DataSourceRequest] DataSourceRequest request, string clientId)
        {
            var folders = await _folderManager.GetClientFoldersAsync(clientId);
            var docTypes = await _folderManager.GetDocumentTypesAsync();
            var model = CreateSharedFolderViewModelList(folders, docTypes);
            var result = model.ToTreeDataSourceResult(request,
                e => e.NodeId,
                e => e.ParentNodeId
            );
            return Json(result);
        }

        [Authorize]
        [HttpPost, Route("/api/sites/shared-folders")]
        public async Task<JsonResult> GetSiteFolders([DataSourceRequest] DataSourceRequest request, string siteId)
        {
            var folders = await _folderManager.GetSiteFoldersAsync(siteId);
            var docTypes = await _folderManager.GetDocumentTypesAsync();
            var model = CreateSharedFolderViewModelList(folders, docTypes);
            var result = model.ToTreeDataSourceResult(request,
                e => e.NodeId,
                e => e.ParentNodeId
            );
            return Json(result);
        }

        [Authorize]
        [HttpPost, Route("/api/clients/folders")]
        public async Task<ActionResult> InsertClientFolder(SharedFolderViewModel model, string clientId)
        {
            if (ModelState.IsValid)
            {
                Folder folder;
                folder = await _folderManager.InsertClientFolderAsync(clientId, model.Title, model.DocumentType, model.ParentFolderId);
                var result = folder.ProjectTo<SharedFolderViewModel>();
                return Ok(result);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/sites/folders")]
        public async Task<ActionResult> InsertSiteFolder(SharedFolderViewModel model, string siteId)
        {
            if (ModelState.IsValid)
            {
                Folder folder;
                folder = await _folderManager.InsertSiteFolderAsync(siteId, model.Title, model.DocumentType, model.ParentFolderId);
                var result = folder.ProjectTo<SharedFolderViewModel>();
                return Ok(result);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPut, Route("/api/folders")]
        public async Task<ActionResult> UpdateFolder(SharedFolderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var folder = new Folder();
                folder.Id = model.FolderId;
                folder.Title = model.Title;
                folder.ParentId = model.ParentFolderId;
                folder.DocumentType = model.DocumentType;
                await _folderManager.UpdateFolderAsync(folder);
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete, Route("/api/folders")]
        public async Task<ActionResult> DeleteFolder(SharedFolderViewModel model)
        {
            if (model != null)
            {
                await _folderManager.DeleteFolder(model.FolderId);
                return Ok(model);
            }
            return BadRequest(ModelState);
        }
    }
}
