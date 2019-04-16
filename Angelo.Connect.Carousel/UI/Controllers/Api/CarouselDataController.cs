using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Angelo.Connect.Carousel.Models;
using Microsoft.AspNetCore.Mvc;
using Angelo.Connect.Widgets;
using Angelo.Connect.Carousel.Services;

namespace Angelo.Connect.Documents.UI.Controllers.Api
{
    [Authorize]
    public class CarouselDataController : Controller
    {
        private IWidgetService<CarouselWidget> _widgetService;
        private CarouselSlideService _carouselSlideService;


        public CarouselDataController(IWidgetService<CarouselWidget> widgetService, CarouselSlideService carouselSlideService)
        {
            _widgetService = widgetService;
            _carouselSlideService = carouselSlideService;


        }

        [HttpPost, Route("/api/widgets/carousel/add")]
        public IActionResult AddItem(CarouselSlide model)
        {
            if (ModelState.IsValid)
            {
                var widget = _widgetService.GetModel(model.WidgetId);

               
                if (widget.Slides.Any(x => x.Id == model.Id))
                {

                    _carouselSlideService.UpdateSlide(model);
                }
                else
                {
                    _carouselSlideService.AddSlide(model);
                }

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete, Route("/api/widgets/carousel/delete")]
        public IActionResult DeleteItem(string id)
        {
            
                var widgetSlide = _carouselSlideService.Get(id);


                if (widgetSlide != null)
                {
                    _carouselSlideService.Delete(widgetSlide);
                }
                
                return Ok();
            
        }

        [HttpPost, Route("/api/widgets/carousel/sortorder")]
        public async Task<IActionResult> UpdateSortOrder(string[] slideIds)
        {
            var sort = 0;
            foreach (var slide in slideIds)
            {
                if (!string.IsNullOrEmpty(slide))
                {
                    sort++;
                    await _carouselSlideService.UpdateSortOrder(slide, sort);
                }
            }

            return Ok();
        }

        //[HttpPost, Route("/api/widgets/documentlist/addFolder")]
        //public async Task<IActionResult> AddNewFolder(string widgetId)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        await _documentListService.AddNewFolder(widgetId);

        //        return Ok();
        //    }

        //    return BadRequest(ModelState);
        //}


        //[HttpPost, Route("/api/widgets/documentlist/title")]
        //public async Task<IActionResult> SaveTitle(string id, string title)
        //{
        //    await _documentListService.SaveTitle(id, title);

        //    return Ok();
        //}

        //[HttpPost, Route("/api/widgets/documentlist/item/url")]
        //public async Task<IActionResult> SaveDocumentUrl(string id, string url)
        //{
        //    await _documentListService.SaveDocumentUrl(id, url);

        //    return Ok();
        //}

        //[HttpDelete, Route("/api/widgets/documentlist")]
        //public async Task<IActionResult> DeleteItem(string id)
        //{
        //    await _documentListService.DeleteItem(id);
        //    ViewBag.DeletedItem = true;
        //    return Ok();
        //}

        //[HttpDelete, Route("/api/widgets/documentlist/folder")]
        //public async Task<IActionResult> DeleteFolder(string id)
        //{
        //    await _documentListService.DeleteFolder(id);
        //    ViewBag.DeletedItem = true;
        //    return Ok();
        //}

        //[HttpPost, Route("/api/widgets/documentlist/item/title")]
        //public async Task<IActionResult> SaveDocumentTitle(string id, string title)
        //{
        //    await _documentListService.SaveDocumentTitle(id, title);

        //    return Ok();
        //}

        //[HttpPost, Route("/api/widgets/documentlist/folder/title")]
        //public async Task<IActionResult> SaveFolderTitle(string id, string title)
        //{
        //    await _documentListService.SaveFolderTitle(id, title);

        //    return Ok();
        //}

        //[HttpPost, Route("/api/widgets/documentlist/sortorder")]
        //public async Task<IActionResult> UpdateSortOrder(string[] fileIds, string parentId)
        //{
        //    var sort = 0;
        //    foreach (var fileId in fileIds)
        //    {
        //        if (!string.IsNullOrEmpty(fileId))
        //        {
        //            sort++;
        //            await _documentListService.UpdateSortOrder(fileId, sort, parentId);
        //        }
        //    }

        //    return Ok();
        //}

        //[HttpPost, Route("/api/widgets/documentlist/folder/sortorder")]
        //public async Task<IActionResult> UpdateSortOrder(string[] folderIds)
        //{
        //    var sort = 0;
        //    foreach (var folderId in folderIds)
        //    {
        //        if (!string.IsNullOrEmpty(folderId))
        //        {
        //            sort++;
        //            await _documentListService.UpdateFolderSortOrder(folderId, sort);
        //        }
        //    }

        //    return Ok();
        //}
    }
}
