using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.Models;

namespace Angelo.Connect.CoreWidgets.UI.Components
{
    public class ContactFormSettings : ViewComponent
    {
        private ContactFormService _contactFormService;

        public ContactFormSettings(ContactFormService contactFormService)
        {
            _contactFormService = contactFormService;
        }

        public async Task<IViewComponentResult> InvokeAsync(ContactForm model)
        {
            return await Task.Run(() =>  View(model));
        }
    }
}
