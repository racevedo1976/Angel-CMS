using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Widgets;
using Angelo.Connect.SlideShow.Models;
using Angelo.Connect.SlideShow.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Angelo.Connect.SlideShow.UI.Controllers.Api
{
    [Authorize]
    public class GalleryDataController : Controller
    {
        private IWidgetService<GalleryWidget> _widgetService;
        private GalleryService _galleryService;

        public GalleryDataController(IWidgetService<GalleryWidget> widgetService, GalleryService galleryService)
        {
            _widgetService = widgetService;
            _galleryService = galleryService;
        }

        [HttpPost, Route("/api/widgets/gallery/add")]
        public async Task<IActionResult> AddItem(GalleryItem model)
        {
            if (ModelState.IsValid)
            {
                //model.WidgetId = widgetId;

                await _galleryService.AddItem(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/gallery/title")]
        public async Task<IActionResult> SaveTitle(string id, string title)
        {
            await _galleryService.SaveTitle(id, title);

            return Ok();
        }

        [HttpDelete, Route("/api/widgets/gallery")]
        public async Task<IActionResult> DeleteItem(string id)
        {
            await _galleryService.DeleteItem(id);
            ViewBag.DeletedItem = true;
            return Ok();
        }

        [HttpPost, Route("/api/widgets/gallery/item/caption")]
        public async Task<IActionResult> SaveItemCaption(string id, string caption)
        {
            await _galleryService.SaveItemCaption(id, caption);

            return Ok();
        }

        [HttpPost, Route("/api/widgets/gallery/captions")]
        public async Task<IActionResult> SaveCaptions(List<GalleryItem> galleryItems)
        {
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }

            var index = 0;
            foreach (var galleryItem in galleryItems)
            {
                if (string.IsNullOrEmpty(galleryItem.Title))
                {
                    ModelState.AddModelError($"galleryitems[{index}].Title", $"Caption is required on image #{index + 1}.");
                }
                else
                {
                    await _galleryService.SaveItemCaption(galleryItem.Id, galleryItem.Title);
                }

                index++;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            return Ok();
        }

        [HttpPost, Route("/api/widgets/gallery/url")]
        public async Task<IActionResult> SaveUrl(string id, string url)
        {
            await _galleryService.SaveUrl(id, url);

            return Ok();
        }
    }
}
