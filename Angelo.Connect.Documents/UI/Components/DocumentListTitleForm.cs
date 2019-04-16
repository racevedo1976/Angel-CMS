using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Documents.Services;
using Angelo.Connect.Documents.Models;

namespace Angelo.Connect.Documents.UI.Components
{
    public class DocumentListTitleForm : ViewComponent
    {
        private DocumentListService _documentService;

        public DocumentListTitleForm(DocumentListService documentService)
        {
            _documentService = documentService;
        }

        public async Task<IViewComponentResult> InvokeAsync(DocumentListWidget model)
        {
            return await Task.Run(() =>  View(model));
        }
    }
}
