using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.CoreWidgets.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.UI.ViewModels;

namespace Angelo.Connect.CoreWidgets.UI.Controllers
{
    [Authorize]
    public class CoreWidgetApiController : Controller
    {
        private HtmlDbContext _htmlDbContext;
        private AlertService _alertService;
        private TitleService _titleService;
        private RawHtmlService _rawHtmlService;
        private ImageService _imageService;
        private IconService _iconService;
        private HeroService _heroService;
        private LightboxService _lightboxService;
        private ContactFormService _contactFormService;

        public CoreWidgetApiController
        (
            HtmlDbContext htmlDbContext, 
            AlertService alertService, 
            TitleService titleService, 
            RawHtmlService rawHtmlService, 
            ImageService imageService, 
            IconService iconService,
            HeroService heroService,
            LightboxService lightboxService,
            ContactFormService contactFormService
        ) 
        {
            _htmlDbContext = htmlDbContext;
            _alertService = alertService;
            _titleService = titleService;
            _rawHtmlService = rawHtmlService;
            _imageService = imageService;
            _iconService = iconService;
            _heroService = heroService;
            _lightboxService = lightboxService;
            _contactFormService = contactFormService;
        }

        [HttpPost, Route("/api/widgets/alert")]
        public IActionResult UpdateAlert(Alert model)
        {
            if (ModelState.IsValid)
            {
                _alertService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/alert/{id}")]
        public IActionResult UpdateAlertInline(string id, [FromForm] string text)
        {
            if (text != null)
            {
                var model = _alertService.GetModel(id);
                model.Text = text;

                _alertService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest("Alert text cannot be empty");
        }

        [HttpPost, Route("/api/widgets/title")]
        public IActionResult UpdateTitle(Title model)
        {
            if (ModelState.IsValid)
            {
                _titleService.UpdateModel(model);

                return Ok(model);
            }
         
            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/title/{id}")]
        public IActionResult UpdateTitleInline(string id, [FromForm] string text)
        {
            if (text != null)
            {
                var model = _titleService.GetModel(id);
                model.Text = text;

                _titleService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest("Title text cannot be empty");
        }

        [HttpPost, Route("/sys/widgets/html")]
        public IActionResult UpdateRawHtml(RawHtml model)
        {
            if (ModelState.IsValid)
            {
                _rawHtmlService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/sys/widgets/html/{id}")]
        public IActionResult UpdateRawHtmlInline(string id, [FromForm] string html)
        {
            var model = _rawHtmlService.GetModel(id);

            if (model != null)
            {
                model.Html = html;
                _rawHtmlService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest($"Could not locate raw html model for Id: {id}");
        }

        [HttpPost, Route("/api/widgets/hero")]
        public IActionResult UpdateHero(HeroUnit model)
        {
            if (ModelState.IsValid)
            {
                _heroService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/hero/{id}/title")]
        public IActionResult UpdateHeroTitleInline(string id, [FromForm] string title)
        {
            var model = _heroService.GetModel(id);

            if (model != null)
            {
                model.Title = title;
                _heroService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest($"Could not locate hero model for Id: {id}");
        }

        [HttpPost, Route("/api/widgets/hero/{id}/body")]
        public IActionResult UpdateHeroBodyInline(string id, [FromForm] string body)
        {
            var model = _heroService.GetModel(id);

            if (model != null)
            {
                model.Body = body;
                _heroService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest($"Could not locate hero model for Id: {id}");
        }

        [HttpPost, Route("/sys/widgets/icon")]
        public IActionResult UpdateIcon(Icon model)
        {
            if (ModelState.IsValid)
            {
                _iconService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/html/image")]
        public IActionResult UpdateImage(Image model)
        {
            if (ModelState.IsValid)
            {
                _imageService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }


        [HttpPost, Route("/api/widgets/html/navbar")]
        public async Task<IActionResult> UpdateNavBar(NavBar model)
        {
            if (ModelState.IsValid)
            {
                var entity = await _htmlDbContext.NavBars.FirstOrDefaultAsync(x => x.Id == model.Id);

                entity.ItemWidth = model.ItemWidth;
                await _htmlDbContext.SaveChangesAsync();

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("/api/widgets/html/navbarmenu")]
        public async Task<IActionResult> UpdateNavBarMenu(NavBar model)
        {
            if (ModelState.IsValid)
            {
                var entity = await _htmlDbContext.NavBars.FirstOrDefaultAsync(x => x.Id == model.Id);

                entity.NavMenuId = model.NavMenuId;
                await _htmlDbContext.SaveChangesAsync();

                return Ok(model);
            }

            return BadRequest(ModelState);
        }


        [HttpPost, Route("/sys/widgets/lightbox")]
        public IActionResult UpdateLightbox(Lightbox model)
        {
            if (model?.TriggerType == "Image" && string.IsNullOrEmpty(model?.Caption))
            {
                ModelState.AddModelError("Caption", "A caption is required for images");
            }

            if (ModelState.IsValid)
            {
                _lightboxService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }


        [HttpPost, Route("/sys/widgets/contactform")]
        public IActionResult ContactFormUpdateSettings(ContactForm model)
        {
            if (string.IsNullOrEmpty(model?.Recipients))
            {
                ModelState.AddModelError("Recipients", "Required");
            }

            if (ModelState.IsValid)
            {
                _contactFormService.UpdateModel(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [AllowAnonymous] 
        [HttpPost, Route("/sys/widgets/contactform/send")]
        public IActionResult ContactFormSendMessage(ContactFormMessage model)
        {
            if(ModelState.IsValid)
            {
                model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                _contactFormService.Send(model);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

    }
}
