using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;

using Angelo.Identity;
using Angelo.Identity.Services;


namespace Angelo.Connect.Services
{
    public class ClientManager
    {
        private ConnectDbContext _db;
        private ConnectCoreOptions _coreOptions;

        private ProductManager _productManager;
        private TenantManager _tenantManager;
        private DirectoryManager _directoryManager;
        private SecurityPoolManager _poolManager;
        private RoleManager _roleManager;

        public ClientManager
        (
            ConnectDbContext dbContext, 
            ConnectCoreOptions coreOptions,
            ProductManager productManger,
            TenantManager tenantManager,
            DirectoryManager directoryManager,
            SecurityPoolManager poolManager,
            RoleManager roleManager
        )
        {
            _db = dbContext;

            _productManager = productManger;
            _tenantManager = tenantManager;
            _directoryManager = directoryManager;
            _poolManager = poolManager;
            _roleManager = roleManager;

            _coreOptions = coreOptions;
        }

        public async Task<ICollection<Client>> GetAll()
        {
            return await _db.Clients.ToListAsync();
        }

        public async Task<Client> GetByIdAsync(string id)
        {
            var client = await _db.Clients.FirstOrDefaultAsync(x => x.Id == id);

            return client;
        }

        public async Task<Client> GetByTenantKeyAsync(string tenantKey)
        {
            var client = await _db.Clients.AsNoTracking().FirstOrDefaultAsync(x => x.TenantKey == tenantKey);

            return client;
        }
    
        public async Task<List<SiteTemplate>> GetTemplatesAsync(string clientId)
        {
            var products = await GetActiveClientProductsAsync(clientId);
            var templates = new List<SiteTemplate>();
            foreach (var product in products)
            {
                var list = await _productManager.GetSiteTemplatesAsync(product.Product);
                foreach (var item in list)
                    if (!templates.Where(t => t.Id == item.Id).Any())
                        templates.Add(item);
            }
            return templates;
        }
       
       
        public async Task<IEnumerable<KeyValuePair<string, string>>> GetClientNameSuggestions(string query)
        {
            var clients = await _db.Clients.AsNoTracking()
                .Where(x => x.Name.Contains(query))
                .Select(x => new { Key = x.Id, Value = x.Name })
                .ToListAsync();

            return clients.Select(x => new KeyValuePair<string, string>(key: x.Key, value: x.Value));

        }

      
        public async Task CreateAsync(Client client)
        {
            Ensure.That(client.Id == null, "Client Id is set internally when creating new clients");
            Ensure.NotNullOrEmpty(client.TenantKey);

            client.Id = KeyGen.NewGuid();

            // Initialize OIDC for this new client 
            var tenant = await _tenantManager.CreateAsync(client.TenantKey, client.Name);
            var directory = await _directoryManager.CreateAsync(client.TenantKey, "Default User Directory");

            // create a new security pool mapped to the default directory
            var clientSecurityPool = await _poolManager.CreateTenantPoolAsync
            (
                client.TenantKey,
                client.Name + " Pool",
                new Identity.Models.Directory[] { directory }
            );

            // Create the Connect Client
            client.SecurityPoolId = clientSecurityPool.PoolId;

            if (client.AnniversaryDate == null)
            {
                client.AnniversaryDate = DateTime.UtcNow;
            }

            _db.Add(client);
            await _db.SaveChangesAsync();

            // Insert default roles
            await CreateDefaultClientLevelRoles(client);
        }

        public async Task UpdateAsync(Client client)
        {
            var entity = await _db.Clients.FirstOrDefaultAsync(x => x.Id == client.Id);

            if (entity == null)
                throw new Exception($"Unable to update client. Client {client.Id} does not exist.");
 
            entity.Name = client.Name;
            entity.PreferredName = client.PreferredName;
            entity.ShortName = client.ShortName;
            entity.Address1 = client.Address1;
            entity.Address2 = client.Address2;
            entity.City = client.City;
            entity.State = client.State;
            entity.Country = client.Country;
            entity.PostalCode = client.PostalCode;
            entity.Notes = client.Notes;
            entity.Active = client.Active;
            entity.AnniversaryDate = client.AnniversaryDate;

            // Update tenant key if changed, or case is changed
            // eg, allow teNANt1 to be changed to tenant1
            if(entity.TenantKey != client.TenantKey)
            {
                await _tenantManager.UpdateKeyAsync(entity.TenantKey, client.TenantKey);

                entity.TenantKey = client.TenantKey;
            }

            await _db.SaveChangesAsync();
        }

        /*
         * Products
         */
        #region Product Management

        public async Task<List<ClientProductApp>> GetActiveClientProductsAsync(string clientId)
        {
            var products = await _db.ClientProductApps.AsNoTracking()
                .Where(cp => (cp.ClientId == clientId) && cp.IsActive)
                .Include(cp => cp.Client)
                .Include(cp => cp.Product)
                .ToListAsync();
            return products;
        }

        // Note: This will return the first ClientProductApp found for the specified client.  For now, a client
        // should only have one product assigned to it.  So, this will work for now.
        public async Task<ProductContext> GetDefaultProductContextAsync(string clientId)
        {
            var app = await _db.ClientProductApps.AsNoTracking()
                .Where(x => x.ClientId == clientId)
                .FirstOrDefaultAsync();
            if (app == null)
                throw new Exception($"No ClientProductApp defined for ClientId:{clientId}");

            return await GetProductContextAsync(app.Id);
        }

        public async Task<ClientProductApp> GetProductAsync(string clientProductAppId)
        {
            var app = await _db.ClientProductApps.AsNoTracking()
                .Where(x => x.Id == clientProductAppId)
                .Include(x => x.Client)
                .Include(x => x.Product)
                .FirstOrDefaultAsync();

            // Remove circular references (This will cause an error during Json serialization)
            if (app.Client != null) app.Client.ClientProductApps = null;
            if (app.Product != null) app.Product.ClientProductApps = null;

            return app;
        }

        public async Task<ICollection<ClientProductApp>> GetProductsAsync(string clientId)
        {
            Ensure.Argument.NotNull(clientId, "clientId");

            var apps = await _db.ClientProductApps.AsNoTracking()
                .Include(x => x.Product)
                .ThenInclude(x => x.Category)
                .Where(x => x.ClientId == clientId && x.IsActive)
                .ToArrayAsync();

            return apps;
        }

        public async Task<List<ProductAddOn>> GetProductAddOnsAsync(string clientProductAppId)
        {
            var addon = await _db.ClientProductAddOns.AsNoTracking()
                .Where(x => x.ClientProductAppId == clientProductAppId)
                .Include(x => x.ProductAddOn)
                .Select(x => x.ProductAddOn)
                .ToListAsync();
            return addon;
        }

        public async Task<ProductContext> GetProductContextAsync(string clientProductAppId)
        {
            var app = await GetProductAsync(clientProductAppId);
            if (app == null)
                throw new Exception($"ClientProductApp record not found (Id:{clientProductAppId})");
            if (app.Product == null)
                throw new Exception($"Product not defined for ClientProductApp (Id:{clientProductAppId})");

            var context = new ProductContext();
            context.AppId = app.Id;
            context.AppTitle = app.Title;
            context.Client = app.Client;
            context.Product = app.Product;
            context.MaxSiteCount = app.MaxSiteCount;

            context.ActiveSiteCount = await _db.Sites.AsNoTracking()
                .Where(x => x.ClientProductAppId == clientProductAppId)
                .CountAsync();

            var schema = _productManager.GetProductSchema(context.Product);
            var addOns = await GetProductAddOnsAsync(clientProductAppId);

            foreach (var addOn in addOns)
            {
                schema.MergeAddon(_productManager.GetProductSchema(addOn));
            }

            context.BaseClientMB = schema.IncClientMB;
            context.PerSiteMB = schema.BaseSiteMB;
            context.Features = schema.Features;
            context.SiteTemplates = schema.SiteTemplates;

            return context;
        }


        public async Task RemoveProductAsync(string clientProductAppId)
        {
            var app = await _db.ClientProductApps.Where(x => x.Id == clientProductAppId).FirstOrDefaultAsync();
            if (app != null)
            {
                if (await _db.Sites.AnyAsync(x => x.ClientProductAppId == clientProductAppId))
                    throw new Exception($"Unable to delete ClientProductApp when assigned to one or more sites [appId: {clientProductAppId}]");

                _db.ClientProductApps.Remove(app);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Inserts the ClientProductApp record if the id is empty, or updates otherwise.
        /// Note: This method does not save the addons.
        /// </summary>
        public async Task AddProductAsync(ClientProductApp model, List<string> addonIds = null)
        {
            var product = await _db.Products.Where(x => x.Id == model.ProductId).FirstOrDefaultAsync();

            if (product == null)
                throw new Exception($"Unable to find product [id: {model.ProductId}]");

            model.Title = product.Name;

            var app = await _db.ClientProductApps.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

            if (app == null)
            {
                // Note: Insert if the id is an empty string.
                if (!string.IsNullOrEmpty(model.Id))
                    throw new Exception($"Unable to find and update ClientProductApp [id: {app.Id}]");

                model.Id = Guid.NewGuid().ToString("N");
                _db.ClientProductApps.Add(model);
            }
            else
            {
                app.ProductId = model.ProductId;
                app.Title = model.Title;
                app.SubscriptionType = model.SubscriptionType;
                app.SubscriptionStartUTC = model.SubscriptionStartUTC;
                app.SubscriptionEndUTC = model.SubscriptionEndUTC;
                app.MaxSiteCount = model.MaxSiteCount;
            }
            if (addonIds != null)
            {
                await UpdateProductAddonsAsync(model.Id, addonIds);
            }

            await _db.SaveChangesAsync();
        }

        private async Task UpdateProductAddonsAsync(string appId, List<string> addonIds)
        {
            var links = await _db.ClientProductAddOns.Where(x => x.ClientProductAppId == appId).ToListAsync();

            // Remove links
            foreach (var link in links)
            {
                if (addonIds.Contains(link.ProductAddOnId) == false)
                {
                    _db.ClientProductAddOns.Remove(link);
                }
            }

            // Add links
            foreach (var addonId in addonIds)
            {
                if (links.Any(x => x.ProductAddOnId == addonId) == false)
                {
                    _db.ClientProductAddOns.Add(new Models.ClientProductAddOn()
                    {
                        ClientProductAppId = appId,
                        ProductAddOnId = addonId
                    });
                }
            }

            await _db.SaveChangesAsync();
        }


        #endregion



        private async Task CreateDefaultClientLevelRoles(Client client)
        {
            Ensure.NotNullOrEmpty(client.SecurityPoolId);

            await _roleManager.CreateAsync(new Identity.Models.Role
            {
                PoolId = client.SecurityPoolId,
                Name = "Community Admins",
                IsLocked = true,
                RoleClaims = new Identity.Models.RoleClaim[]
                {
                    new Identity.Models.RoleClaim
                    {
                        ClaimType = SiteClaimTypes.SitePrimaryAdmin,
                        ClaimValue = client.Id
                    },
                    new Identity.Models.RoleClaim
                    {
                        ClaimType = ClientClaimTypes.PrimaryAdmin,
                        ClaimValue = client.Id
                    },
                }
            });
        }
    }
}