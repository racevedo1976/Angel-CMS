using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Widgets;
using Angelo.Connect.Documents.Models;
using Angelo.Connect.Documents.Services;

namespace Angelo.Connect.Documents.UI.Controllers.Api
{
    [Authorize]
    public class DocumentListDataController : Controller
    {
        private IWidgetService<DocumentListWidget> _widgetService;
        private DocumentListService _documentListService;

        public DocumentListDataController(IWidgetService<DocumentListWidget> widgetService, DocumentListService documentListService)
        {
            _widgetService = widgetService;
            _documentListService = documentListService;
        }

        [HttpPost, Route("/api/widgets/documentlist/add")]
        public async Task<IActionResult> AddItem(DocumentListDocument model)
        {
            if (ModelState.IsValid)
            {
                //model.WidgetId = widgetId;

                await _documentListService.AddItem(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }


        [HttpPost, Route("/api/widgets/documentlist/addFolder")]
        public async Task<IActionResult> AddNewFolder(string widgetId)
        {
            if (ModelState.IsValid)
            {
                
                await _documentListService.AddNewFolder(widgetId);

                return Ok();
            }

            return BadRequest(ModelState);
        }


        [HttpPost, Route("/api/widgets/documentlist/title")]
        public async Task<IActionResult> SaveTitle(string id, string title)
        {
            await _documentListService.SaveTitle(id, title);

            return Ok();
        }

        [HttpPost, Route("/api/widgets/documentlist/item/url")]
        public async Task<IActionResult> SaveDocumentUrl(string id, string url)
        {
            await _documentListService.SaveDocumentUrl(id, url);

            return Ok();
        }

        [HttpDelete, Route("/api/widgets/documentlist")]
        public async Task<IActionResult> DeleteItem(string id)
        {
            await _documentListService.DeleteItem(id);
            ViewBag.DeletedItem = true;
            return Ok();
        }

        [HttpDelete, Route("/api/widgets/documentlist/folder")]
        public async Task<IActionResult> DeleteFolder(string id)
        {
            await _documentListService.DeleteFolder(id);
            ViewBag.DeletedItem = true;
            return Ok();
        }

        [HttpPost, Route("/api/widgets/documentlist/item/title")]
        public async Task<IActionResult> SaveDocumentTitle(string id, string title)
        {
            await _documentListService.SaveDocumentTitle(id, title);

            return Ok();
        }

        [HttpPost, Route("/api/widgets/documentlist/folder/title")]
        public async Task<IActionResult> SaveFolderTitle(string id, string title)
        {
            await _documentListService.SaveFolderTitle(id, title);

            return Ok();
        }

        [HttpPost, Route("/api/widgets/documentlist/sortorder")]
        public async Task<IActionResult> UpdateSortOrder(string[] fileIds, string parentId)
        {
            var sort = 0;
            foreach (var fileId in fileIds)
            {
                if (!string.IsNullOrEmpty(fileId))
                {
                    sort++;
                    await _documentListService.UpdateSortOrder(fileId, sort, parentId);
                }
            }

            return Ok();
        }

        [HttpPost, Route("/api/widgets/documentlist/folder/sortorder")]
        public async Task<IActionResult> UpdateSortOrder(string[] folderIds)
        {
            var sort = 0;
            foreach (var folderId in folderIds)
            {
                if (!string.IsNullOrEmpty(folderId))
                {
                    sort++;
                    await _documentListService.UpdateFolderSortOrder(folderId, sort);
                }
            }

            return Ok();
        }
    }
}
