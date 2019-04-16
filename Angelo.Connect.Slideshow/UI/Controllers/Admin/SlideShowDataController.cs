using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Widgets;
using Angelo.Connect.SlideShow.Models;
using Angelo.Connect.SlideShow.Services;

namespace Angelo.Connect.SlideShow.UI.Controllers.Api
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class SlideShowDataController : Controller
    {
        private IWidgetService<SlideShowWidget> _widgetService;
        private IDocumentService<Slide> _documentService;
        private SlideShowService _slideShowService;

        public SlideShowDataController(IWidgetService<SlideShowWidget> widgetService, IDocumentService<Slide> documentService, SlideShowService slideShowService)
        {
            _widgetService = widgetService;
            _documentService = documentService;
            _slideShowService = slideShowService;
        }

        [HttpPost, Route("/api/widgets/slideshow/title")]
        public IActionResult UpdateWidgetTitle(SlideShowWidget model, string templateId = null)
        {
            if (string.IsNullOrEmpty(templateId)) templateId = null;

            if (ModelState.IsValid)
            {
                var existing = _widgetService.GetModel(templateId ?? model.Id);
                
                if (existing != null)
                {
                    existing.Title = model.Title;
                    existing.Description = model.Description;
                    existing.Duration = model.Duration;
                    existing.BackgroundColor = model.BackgroundColor;
                    existing.Transition = model.Transition;
                    existing.Height = model.Height;
                }
                //if (!string.IsNullOrEmpty(model.SlideShowId))
                //{
                //    existing.SlideShow = _slideShowService.Get(model.SlideShowId);
                //    existing.SlideShowId = existing.SlideShow?.Id;

                //    existing.SlideShow.Title = model.SlideShow?.Title ?? existing.SlideShow?.Title;
                //    existing.SlideShow.Description = model.SlideShow?.Description ?? existing.SlideShow?.Description;
                //}
                //else if (!string.IsNullOrEmpty(templateId))
                //{
                //    existing.SlideShow = _slideShowService.Get(templateId);
                //    existing.SlideShow.Id = existing.SlideShowId = KeyGen.NewGuid();
                //}
                //else if (!string.IsNullOrEmpty(existing.SlideShowId))
                //{
                //    existing.SlideShow = _slideShowService.Get(model.SlideShowId);
                //}
                //else
                //{
                //    existing.SlideShow = new Models.SlideShow();

                //    existing.SlideShowId = existing.SlideShow.Id = KeyGen.NewGuid();
                //    existing.SlideShow.Title = model.SlideShow.Title;
                //    existing.SlideShow.Description = model.SlideShow.Description;
                //}

                _widgetService.UpdateModel(existing);    // Entity Framework isn't letting me attach a subclass

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/slideshow/defaults")]
        public IActionResult UpdateWidgetDefaults(SlideShowWidget model)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    var existing = _widgetService.GetModel(model.Id);

                    // Copy type properties to the existing widget
                    //existing.DefaultProgressBarColor = model.DefaultProgressBarColor;
                    //existing.DefaultProgressBarHeight = model.DefaultProgressBarHeight;
                    //existing.DefaultProgressBarIsActive = model.DefaultProgressBarIsActive;
                    //existing.DefaultProgressBarOpacity = model.DefaultProgressBarOpacity;
                    //existing.DefaultProgressBarPosition = model.DefaultProgressBarPosition; 
                    //existing.DefaultSlideShowBackgroundColor = model.DefaultSlideShowBackgroundColor;
                    //existing.DefaultSlideShowDottedOverlaySize = model.DefaultSlideShowDottedOverlaySize;
                    //existing.DefaultSlideShowDuration = model.DefaultSlideShowDuration;
                    //existing.DefaultSlideShowImageSourceSize = model.DefaultSlideShowImageSourceSize;
                    //existing.DefaultSlideShowImageUrl = model.DefaultSlideShowImageUrl;
                    //existing.DefaultSlideShowInitializationDelay = model.DefaultSlideShowInitializationDelay;
                    //existing.DefaultSlideShowIsLooped = model.DefaultSlideShowIsLooped;
                    //existing.DefaultSlideShowIsPausedOnHover = model.DefaultSlideShowIsPausedOnHover;
                    //existing.DefaultSlideShowIsRandomMode = model.DefaultSlideShowIsRandomMode;
                    //existing.DefaultSlideShowMaximumLoops = model.DefaultSlideShowMaximumLoops;
                    //existing.DefaultSlideShowPadding = model.DefaultSlideShowPadding;
                    //existing.DefaultSlideShowShadowType = model.DefaultSlideShowShadowType;
                    //existing.DefaultSlideShowTransitions = model.DefaultSlideShowTransitions;

                    _widgetService.UpdateModel(existing);    // Entity Framework isn't letting me attach a subclass

                    return Ok(existing);
                }

                return BadRequest(ModelState);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/slideshow/type")]
        public IActionResult UpdateWidgetType(SlideShowWidget model)
        {
            if (ModelState.IsValid)
            {
                var existing = _widgetService.GetModel(model.Id);
                //if (string.IsNullOrEmpty(existing.SlideShowId))
                //{
                //    // The widget has not been created yet (or is being created in this request), so this data should
                //    // be ignored
                //    return Ok(existing);
                //}
                //else
                //{
                //    existing.SlideShow = _slideShowService.Get(existing.SlideShowId);
                //}

                //// Copy type properties to the existing widget
                //existing.SlideShow.Type = model.SlideShow.Type;
                //existing.SlideShow.FormHeight = model.SlideShow.FormHeight;
                //existing.SlideShow.FormWidth = model.SlideShow.FormWidth;

                _widgetService.UpdateModel(existing);    // Entity Framework isn't letting me attach a subclass
                
                return Ok(existing);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/slideshow/slide")]
        public async Task<IActionResult> UpdateWidgetSlide(Slide model, string slideSubmit)
        {
            if (slideSubmit == "false")
                return Ok();

            if (ModelState.IsValid)
            {
                //empty, object. Framework is posting all form in the modal window.
                if (string.IsNullOrEmpty(model.DocumentId))
                {
                    return Ok(model);
                }

                // TODO: Update Thumbnail URL
                var slide = await _documentService.GetAsync(model.DocumentId);

                if (slide == null)
                {
                    //get the last position
                    model.Position = _slideShowService.GetSlidesMaxPosition(model.WidgetId) + 1;
                    model.Title = model.Title ?? "";
                    model.ImageUrl = model.ImageUrl ?? "";
                    await _documentService.CreateAsync(model);
                }else
                {
                    slide.Title = model.Title ?? "";
                    slide.Duration = model.Duration;
                    slide.ImageUrl = model.ImageUrl ?? "";
                    slide.UseVideoBackground = model.UseVideoBackground;
                    slide.VideoUrl = model.VideoUrl ?? "";
                    slide.VideoSource = model.VideoSource ?? "";
                    slide.EnableVideoSound = model.EnableVideoSound;
                    slide.Transition = model.Transition ;
                    slide.Description = model.Description;
                    slide.Color = model.Color;
                    
                    await _documentService.UpdateAsync(slide);
                }

                //await _documentService.UpdateAsync(model);    // Entity Framework isn't letting me attach a subclass

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpGet, Route("/api/widgets/slideshow/deleteslide")]
        public async Task<IActionResult> DeleteSlide(string id)
        {
            if (ModelState.IsValid)
            {

                var slide = _documentService.Query().FirstOrDefault(x => x.DocumentId == id);

                if (slide != null)
                {
                    //remove all layers
                    try
                    {
                        if (slide.Layers != null)
                        {
                            foreach (var layer in slide.Layers.ToList())
                            {
                                _slideShowService.DeleteLayer(layer);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                    }
                    
                    
                    //remove slide
                    await _documentService.DeleteAsync(id);
                }

                return Ok();
            }

            return BadRequest(ModelState);
        }


        [HttpPost, Route("/api/widgets/slideshow/layer")]
        public async Task<IActionResult> UpdateSlideLayer(SlideLayer model, string layerSubmit)
        {
            if (layerSubmit == "false")
                return Ok();

            if (ModelState.IsValid)
            {
                //the ui might need to change. The modal window is posting all
                //form elements which triggers the layer form to be posted without data.
                //TODO  change the ui to only render the form element within the component.
                if (string.IsNullOrEmpty(model.Id))
                {
                    return Ok();
                }

                var layer = await _slideShowService.GetLayerAsync(model.Id);

                if (layer == null)
                {
                    _slideShowService.SaveLayer(model);
                }
                else
                {
                    //TODO remove the "not null" on table creation and remove the following defaults.
                    layer.Title = model.Title;
                    layer.HorizontalAlignment = model.HorizontalAlignment;
                    layer.VerticalAlignment = Alignment.Center;
                    layer.Color = model.Color;
                    layer.SourceUrl = model.SourceUrl;
                    layer.FontFamily = model.FontFamily;
                    layer.FontSize = model.FontSize;
                    layer.X = model.X;
                    layer.Y = model.Y;
                    layer.Transition = model.Transition;
                    layer.Position = model.Position;
                    layer.LayerType = model.LayerType;
                    layer.Delay = model.Delay;
                    layer.Target = model.Target;
                    layer.FontWeight = model.FontWeight;
                    layer.FontStyle = model.FontStyle;
                    layer.BgColor = model.BgColor;
                    layer.TextDecoration = model.TextDecoration;
                    //layer.FadeInTransition = Transition.Blinds;
                    //layer.FadeInDirection = Direction.BottomToTop;
                    //layer.FadeInDuration = 1;
                    //layer.FadeOutDelay = 1;
                    //layer.FadeOutDirection = Direction.BottomToTop;
                    //layer.FadeOutDuration = 1;
                    //layer.FadeOutTransition = Transition.Blinds;

                    _slideShowService.UpdateLayer(layer);
                }

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpGet, Route("/api/widgets/slideshow/deleteslidelayer")]
        public async Task<IActionResult> DeleteSlideLayer(string id)
        {
            if (ModelState.IsValid)
            {

                var layer = await _slideShowService.GetLayerAsync(id);

                if (layer != null)
                {
                    _slideShowService.DeleteLayer(layer);
                }
              

                return Ok();
            }

            return BadRequest(ModelState);
        }

        [HttpGet, Route("/api/widgets/slideshow/slidelist")]
        public IActionResult SlideListComponent(string id)
        {
            return ViewComponent("SlideList", new { widgetId = id });
        }

        [HttpGet]
        //[HttpGet, Route("/api/widgets/slideshow/editslide")]
        public IActionResult EditSlideComponent(string widgetId, string id)
        {
            return ViewComponent("SlideEditForm", new { widgetId = widgetId, slideId = id });
        }

        [HttpGet, Route("/api/widgets/slideshow/updateslideposition")]
        public async void EditSlidePosition(string id, int index)
        {
            if (ModelState.IsValid)
            {
                // TODO: Update Thumbnail URL
                var slide = await _documentService.GetAsync(id);

                if (slide != null)
                {
                    slide.Position = index;
                    
                    await _documentService.UpdateAsync(slide);
                }
            }

        }

        //private void CreateSlideShow(SlideShowWidget model, string templateId)
        //{
        //    var id = KeyGen.NewGuid();
        //    var slideShow = string.IsNullOrEmpty(templateId)
        //        ? new Models.SlideShow()
        //        : _slideShowService.Get(templateId);
        //    slideShow.Id = id;

        //    model.SlideShow = slideShow;
        //    model.SlideShowId = id;
        //}

        //     private void UpdateWidgetProperties(SlideShowWidget model, string slideShowId)
        //     {
        //         UpdateSlideShowDefaults(model, slideShowId);
        //     }

        //     private void UpdateSlideShowDefaults(SlideShowWidget widget, string slideShowId)
        //     {
        //         UpdateSlideShowDefaults(widget, _slideShowService.Get(slideShowId));
        //     }

        //     private void UpdateSlideShowDefaults(SlideShowWidget widget, Models.SlideShow slideShow)
        //     {
        //         if (slideShow == null) return;

        //         widget.DefaultSlideShowBackgroundColor = slideShow.BackgroundColor;
        //         widget.DefaultSlideShowDuration = slideShow.Duration;
        ////         widget.DefaultSlideShowImageSourceSize = slideShow.ImageSourceSize;
        //         widget.DefaultSlideShowImageUrl = slideShow.ImageUrl;
        //         widget.DefaultSlideShowInitializationDelay = slideShow.InitializationDelay;
        //         widget.DefaultSlideShowIsLooped = slideShow.IsLooped;
        //         widget.DefaultSlideShowIsPausedOnHover = slideShow.IsPausedOnHover;
        //         widget.DefaultSlideShowIsRandomMode = slideShow.IsRandomMode;
        //         widget.DefaultSlideShowMaximumLoops = slideShow.MaximumLoops;
        //         widget.DefaultSlideShowPadding = slideShow.Padding;
        //         widget.DefaultSlideShowTransitions = slideShow.Transitions;
        //     }
    }
}
