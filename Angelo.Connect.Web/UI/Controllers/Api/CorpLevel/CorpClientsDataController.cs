using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper.Extensions;

using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Angelo.Connect.Extensions;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Documents;

namespace Angelo.Connect.Web.UI.Controllers
{
    public class CorpClientsDataController : BaseController
    {
        private ClientManager _clientManager;
        private SiteManager _siteManager;
        private ProductManager _productManger;
        private Identity.SecurityPoolManager _poolManager;
        private IFolderManager<FileDocument> _folderManager;

        public CorpClientsDataController(
            SiteManager siteManager,
            ClientManager clientManager, 
            ProductManager productManger,
            Identity.SecurityPoolManager poolManager,
            IFolderManager<FileDocument> folderManager,
            ILogger<CorpClientsDataController> logger) : base(logger)
        {
            _siteManager = siteManager;
            _clientManager = clientManager;
            _productManger = productManger;
            _poolManager = poolManager;
            _folderManager = folderManager;
        }

        [Authorize(policy: PolicyNames.CorpClientsRead)]
        [HttpPost, Route("/sys/corp/api/clients/data")]
        public async Task<JsonResult> Data([DataSourceRequest]DataSourceRequest request, string clientId = null)
        {
            var result = await _clientManager.GetAll();

            var clients = result.ProjectTo<ClientViewModel>();

            ViewData["ClientId"] = clientId;

            return Json(clients.ToDataSourceResult(request));
        }

        [Authorize(policy: PolicyNames.CorpClientsRead)]
        [HttpGet, Route("/sys/corp/api/clients/{clientId}/sites")]
        public async Task<JsonResult> GetClientSites(string clientId)
        {
            var sites = await _siteManager.GetByClientIdAsync(clientId);
            var model = sites.ProjectTo<SiteViewModel>();

            return Json(model);
        }

        [Authorize(policy: PolicyNames.CorpClientsRead)]
        [HttpPost, Route("/sys/corp/api/clients/{clientId}/sites/data")]
        public async Task<JsonResult> GetClientSitesGridData([DataSourceRequest]DataSourceRequest request, string clientId)
        {
            var sites = await _siteManager.GetByClientIdAsync(clientId);
            var model = sites.ProjectTo<SiteViewModel>();

            return Json(model.ToDataSourceResult(request));
        }

        [Authorize(policy: PolicyNames.CorpClientsCreate)]
        [HttpPost, Route("/sys/corp/api/clients")]
        public async Task<ActionResult> SaveClient(Client model)
        {
            if (ModelState.IsValid)
            {
                model.TenantKey = model.TenantKey.FormatTenantKey();

                // Make sure that the entered tenant key does not already exist.
                var keyClient = await _clientManager.GetByTenantKeyAsync(model.TenantKey);
                if ((keyClient != null) && (keyClient.Id != model.Id))
                {
                    ModelState.AddModelError("TenantKey", "Please enter a unique key.");
                    return BadRequest(ModelState);
                }

                var client = model.ProjectTo<Client>();
                if (string.IsNullOrEmpty(model.Id))
                {
                
                    await _clientManager.CreateAsync(client);

                    model.SecurityPoolId = client.SecurityPoolId;          
                }
                else
                {
                    await _clientManager.UpdateAsync(client);
                }

                //ensure a library is created with a root folder
                CreateLibraryAndRootFolder(client);

                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        private async void CreateLibraryAndRootFolder(Client client)
        {
            var locationResolver = new DocumentPhysicalLocationResolver("Client", client.Id, "", "");

            //creates the document library
            var documentLibrary = await _folderManager.CreateDocumentLibrary(client.Id, "Client", locationResolver.Resolve());

            //create root folder / ensure there is a root folder
            await _folderManager.GetRootFolderAsync(client.Id);
        }
    }
}
