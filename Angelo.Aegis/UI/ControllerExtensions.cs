using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Aegis.UI.ViewModels;


namespace Angelo.Aegis.UI
{
    public static class ControllerExtensions
    {

        public static ViewResult ErrorView(this Controller controller, string errorMessage)
        {
            var error = new IdentityServer4.Models.ErrorMessage()
            {
                Error = errorMessage
            };

            return controller.View("Error", new ErrorViewModel() { Error = error });
        }
    }
}
