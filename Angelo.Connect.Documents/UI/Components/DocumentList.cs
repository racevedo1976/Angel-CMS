using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Documents.Services;
using Angelo.Connect.Documents.Models;

namespace Angelo.Connect.Documents.UI.Components
{
    public class DocumentList : ViewComponent
    {
        private DocumentListService _documentListService;

        public DocumentList(DocumentListService documentListService)
        {
            _documentListService = documentListService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId)
        {
            var model = new DocumentListWidget();
            model.Id = widgetId;
            return await Task.Run(() =>  View(model));
        }
    }
}
