using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Models;
using Angelo.Connect.Security;

namespace Angelo.Connect.Video.UI.Controllers.Admin
{
    [RequireFeature(FeatureId.LiveVideo)]
    public class VideoStreamLinkController : Controller
    {
        private SiteContext _siteContext;
        private AdminContext _menuContext;


        public VideoStreamLinkController(SiteContext siteContext, AdminContext menuContext)
        {
            _siteContext = siteContext;
            _menuContext = menuContext;
        }

        public IActionResult Index()
        {
            ViewData["clientId"] = _menuContext.ClientId;

            var isLiveVideoEnabled = _siteContext.ProductContext.Features.Get(FeatureId.LiveVideo)?.GetSettingValue<bool>("enabled") ?? false;

            if (isLiveVideoEnabled)
            {
                return View("~/UI/Views/Admin/VideoStreams.cshtml");
            }else
            {
                return View("AccessDenied");
            }
        }
    }
}
