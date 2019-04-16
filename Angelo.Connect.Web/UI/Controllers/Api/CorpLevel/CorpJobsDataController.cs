using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using Angelo.Jobs;
using Angelo.Connect.Logging;
using Angelo.Connect.Services;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class CorpJobsDataController : Controller
    {
        private IJobsManager _jobs;
        private DbLogService _logger;
        private DbLogContext _db;

        public CorpJobsDataController(IJobsManager jobs, DbLogService logger, DbLogContext db)
        {
            _jobs = jobs;
            _logger = logger;
            _db = db;
        }    

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpPost, Route("/sys/jobs/log")]
        public IActionResult FetchLogEvents([DataSourceRequest]DataSourceRequest request)
        {
            // only show info level & above events in job summary
            // lower level will be shown by details
            var eventQuery = _db.Events
                .Where(x => 
                    x.Category.StartsWith("Jobs", StringComparison.OrdinalIgnoreCase)
                    && x.LogLevel >= Microsoft.Extensions.Logging.LogLevel.Information
                )
                .OrderByDescending(x => x.Created);


            return Json(eventQuery.ToDataSourceResult(request));
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        // Kendo Grid View
        [HttpPost, Route("/sys/jobs/log/{id}")]
        public IActionResult GetLogDetails([DataSourceRequest]DataSourceRequest request, string id)
        {           
            var eventQuery = _db.Events.Where(x => x.ResourceId == id);

            return Json(eventQuery.AsQueryable().ToDataSourceResult(request));
        }

        // HttpResponse (realtime) View
        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpGet, Route("/sys/jobs/log/{id}")]
        public async Task StreamLogDetails(string id)
        {
            Response.StatusCode = 200;
            Response.ContentType = "text/html";
            
            await Response.WriteAsync("<!DOCTYPE html>");
            await Response.WriteAsync("<html><body style=\"font-family: 'Courier New'; padding: 15px;\">");
            await Response.Body.FlushAsync();

            DateTime stopTime = DateTime.Now.AddSeconds(30);
            int lastId = 0;

            do
            {
                var query = _db.Events
                    .Where(x => x.ResourceId == id && x.Id > lastId)
                    .OrderBy(x => x.Id)
                    .ToList();

                // default time to wait between queries 
                int delay = 1000;

                if(query != null && query.Count > 0)
                {
                    foreach (var result in query)
                    {
                        await Response.WriteAsync("<div>" + result.Message + "</div>");
                        await Response.Body.FlushAsync();
                        lastId = result.Id;
                    }
                    await Response.WriteAsync("");
                    await Response.Body.FlushAsync();

                    // shorten delay when records are present
                    delay = 50;
                }

                await Response.WriteAsync("");
                await Response.Body.FlushAsync();

                Thread.Sleep(delay);
            }
            while (DateTime.Now < stopTime);

            await Response.WriteAsync("</body></html>");
            await Response.Body.FlushAsync();
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpPost, Route("/sys/jobs/test")]
        public IActionResult RunTestJob()
        {
            var processId = Guid.NewGuid().ToString();

            _jobs.EnqueueAsync<DbLogService>(
                service => service.SetCategory("Jobs", processId).LogAsync("Executed Test Job @ " + DateTime.Now.ToString("hh:mm:ss.fffff"))
            );

            return Ok(processId);
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpGet, Route("/sys/jobs/export")]
        public async Task<IActionResult> GetExportForm([FromServices] SiteManager siteManager)
        {
            var sites = await siteManager.GetByClientIdAsync(DbKeys.ClientIds.PcMac);

            return PartialView("/UI/Views/Admin/CorpAdmin/Jobs/ExportForm.cshtml", sites);
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpPost, Route("/sys/jobs/export")]
        public IActionResult RunExportJob([FromForm] string siteId, [FromForm] string templateId, [FromForm] string templateTitle)
        {
            var processId = Guid.NewGuid().ToString();

            _jobs.EnqueueAsync<SiteTemplateExporter>(
                exporter => exporter.ExportSiteAsTemplate(siteId, templateId, templateTitle, processId)
            );

            return Ok(processId);
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpGet, Route("/sys/jobs/lucene")]
        public async Task<IActionResult> GetLuceneForm([FromServices] ClientManager clientManager)
        {
            var clients = await clientManager.GetAll();

            return PartialView("/UI/Views/Admin/CorpAdmin/Jobs/LuceneForm.cshtml", clients);
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpPost, Route("/sys/jobs/lucene")]
        public async Task<IActionResult> RunLuceneJob([FromServices] SiteManager siteManager, [FromForm] string siteId)
        {
            var processId = Guid.NewGuid().ToString();
            var site = await siteManager.GetByIdAsync(siteId);

            await _jobs.EnqueueAsync<SitePublisher>(
                publisher => publisher.QueueSearchIndex(site)
            );

            _logger.SetCategory("Jobs.Lucene", processId);
            _logger.Log($"Queued {site.Title} with external crawler service.");

            return Ok(processId);
        }


    }
}
