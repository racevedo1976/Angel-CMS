using Angelo.Connect.Logging;
using Angelo.Connect.Models;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components
{
    public class DocumentLog : ViewComponent
    {
        private DbLoggerProvider _logger;
        public DocumentLog(DbLoggerProvider logger)
        {
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string documentId, string userId)
        {
            //var url = $"http://localhost:24000/api/doc/log?id={documentId}";

            //var model = JsonConvert.DeserializeObject<LogDescriptor>(await new HttpClient().GetStringAsync(url));
            //var model = new List<object>();
            var model = _logger.CreateLogger(typeof(FileDocument).Name, documentId);

            return await Task.Run(() => View(model));
        }
    }
}
