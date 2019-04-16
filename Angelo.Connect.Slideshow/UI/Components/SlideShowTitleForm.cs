using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.SlideShow.Services;
using Angelo.Connect.SlideShow.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Configuration;
using System.Security.Claims;
using Angelo.Connect.Security;

namespace Angelo.Connect.SlideShow.UI.Components
{
    public class SlideShowTitleForm : ViewComponent
    {
        private SlideShowService _slideShowService;
        private UserContext _userContext;
        public SlideShowTitleForm(SlideShowService slideShowService, UserContext userContext)
        {
            _slideShowService = slideShowService;
            _userContext = userContext;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(SlideShowWidget model)
        {
            return await Task.Run(() => View(model));
        }
    }
}
