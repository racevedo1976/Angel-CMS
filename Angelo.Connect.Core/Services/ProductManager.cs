using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

using Angelo.Common.Models;
using Angelo.Common.Extensions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;
using Angelo.Connect.Extensions;

namespace Angelo.Connect.Services
{
    public class ProductManager
    {
        private ConnectDbContext _db;
        private ConnectCoreOptions _options;
        private SiteTemplateManager _templateManager;

        public ProductManager(ConnectDbContext dbContext, ConnectCoreOptions options, SiteTemplateManager templateManager)
        {
            _db = dbContext;
            _options = options;
            _templateManager = templateManager;
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            var product = await _db.Products
                .FirstOrDefaultAsync(x => x.Id == productId);
            return product;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _db.Products.ToListAsync();
        }

        public async Task<List<Product>> GetActiveProductsAsync()
        {
            return await _db.Products.Where(c => c.Active == true).ToListAsync();
        }

        public async Task<PagedResult<Product>> GetActiveProductsByPageAsync(int pageNumber, int pageSize = 20)
        {
            return await _db.Products
                .Where(c => c.Active == true)
                .OrderBy(x => x.CategoryId)
                .PagedResultAsync(pageNumber, pageSize);
        }

        public async Task<ProductAddOn> GetProductAddOnById(string productAddOnId)
        {
            return await _db.ProductAddOns.AsNoTracking()
                .Where(x => x.Id == productAddOnId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ProductAddOn>> GetAddOnsOfProductId(string ProductId)
        {
            return await _db.ProductAddOnLinks.AsNoTracking()
                .Where(l => l.ProductId == ProductId)
                .Include(x => x.ProductAddOn)
                .Select(x => x.ProductAddOn)
                .ToListAsync();
        }

        /// <summary>
        /// Returns the root path with the specified Filename appended to it.
        /// </summary>
        protected string ComposeConentPath(string filename)
        {
            string rootPath = _options.FileSystemRoot.Replace('/', '\\');
            string filePath = rootPath.TrimEnd('\\') + "\\" + filename.Replace('/', '\\').TrimStart('\\');
            return filePath;
        }

        protected JObject LoadJObjectFromFile(string filename)
        {
            var filePath = ComposeConentPath(filename);
            if (File.Exists(filePath) == false)
                throw new Exception($"Product file not found: {filePath}");
            var projectText = System.IO.File.ReadAllText(filePath);
            var jObject = JObject.Parse(projectText);
            return jObject;
        }

        protected List<Feature> GetFeaturesFromJObject(JObject jObject)
        {
            var features = new List<Feature>();
            var featuresSection = jObject.Property("Features");
            if (featuresSection != null)
            {
                foreach (var item in featuresSection.Value)
                {
                    features.Add(new Feature()
                    {
                        Id = (item["Id"] == null) ? ((item["Name"] == null) ? null : item["Name"].Value<string>()) : item["Id"].Value<string>(),
                        Type = item["Type"]?.Value<string>(),
                        Title = item["Title"]?.Value<string>(),
                        Settings = item["Settings"]
                    });
                }
            }
            return features;
        }

        public List<string> GetProductWidgets(ProductContext productContext)
        {
            var productWidgetSettings = (JObject)productContext.Features.Get("Widgets").Settings;
            var productWidgetViews = ((JArray)productWidgetSettings["PageWidgets"]).ToObject<List<string>>();

            return productWidgetViews;
        }

        protected List<string> GetWidgetsFromJObject(JObject jObject)
        {
            var productWidgets = new List<string>();
            var widgetsSection = jObject.Property("Features");
            var count = 0;
            if (widgetsSection != null)
            {
                foreach (var item in widgetsSection.Value)
                {
                    if (item["Name"].Value<string>() == "Widgets")
                    {
                        productWidgets = ((JArray)jObject["Features"][count]["Settings"]["PageWidgets"]).ToObject<List<string>>();
                    }
                    count = count + 1;
                }
            }
            return productWidgets;
        }


        protected List<SiteTemplate> GetSiteTemplatesFromJObjectAsync(JObject jObject)
        {
            var templates = new List<SiteTemplate>();
            var templateSection = jObject.Property("SiteTemplates");
            if (templateSection != null)
            {
                var templateIds = templateSection.Value.ToObject<List<string>>();
                templates = _templateManager.GetAll()
                    .Where(t => templateIds.Contains(t.Id))
                    .ToList();
            }
            return templates;
        }

        protected ProductSchema LoadProductSchemaFromFile(string filename)
        {
            int intBuffer;
            var jObject = LoadJObjectFromFile(filename);
            var schema = new ProductSchema();
            schema.Id = jObject["ProductId"]?.Value<string>();
            schema.Type = jObject["Type"]?.Value<string>();
            schema.Title = jObject["Title"]?.Value<string>();
            schema.Version = jObject["Version"]?.Value<string>();
            if (Int32.TryParse(jObject["BaseSiteMB"]?.Value<string>(), out intBuffer))
                schema.BaseSiteMB = intBuffer;
            if (Int32.TryParse(jObject["IncClientMB"]?.Value<string>(), out intBuffer))
                schema.IncClientMB = intBuffer;
            schema.Features.AddRange(GetFeaturesFromJObject(jObject));
            schema.SiteTemplates.AddRange(GetSiteTemplatesFromJObjectAsync(jObject));
            return schema;
        }

        public ProductSchema GetProductSchema(Product product)
        {
            try
            {
                return LoadProductSchemaFromFile(product.SchemaFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid Product file format (filename: {product.SchemaFile})", ex);
            }
        }

        public ProductSchema GetProductSchema(ProductAddOn addOn)
        {
            try
            {
                return LoadProductSchemaFromFile(addOn.SchemaFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid Product Add-On file format (filename: {addOn.SchemaFile})", ex);
            }
        }

        public async Task<List<SiteTemplate>> GetSiteTemplatesAsync(Product product)
        {
            try
            {
                var jObject = LoadJObjectFromFile(product.SchemaFile);
                return GetSiteTemplatesFromJObjectAsync(jObject);
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid Product file format (filename: {product.SchemaFile})", ex);
            }
        }

        public async Task<List<SiteTemplate>> GetSiteTemplatesAsync(ProductAddOn addOn)
        {
            try
            {
                var jObject = LoadJObjectFromFile(addOn.SchemaFile);
                return GetSiteTemplatesFromJObjectAsync(jObject);
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid Product Add-On file format (filename: {addOn.SchemaFile})", ex);
            }
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetProductNameSuggestions(string query)
        {
            var products = await _db.Products.AsNoTracking()
                .Where(x => x.Name.Contains(query))
                .Select(x => new { Key = x.Id, Value = x.Name })
                .ToListAsync();

            return products.Select(x => new KeyValuePair<string, string>(key: x.Key, value: x.Value));
        }


        //protected async Task<ClientProduct> InternalCreateClientProductAsync(string productId, bool fullyPopulate = false)
        //{
        //    Ensure.NotNull(productId);

        //    // Make sure that the requested product exists.
        //    Angelo.Connect.Models.Product product = await _db.Products
        //        .Include(x => x.Category)
        //        .Where(p => p.Id == productId)
        //        .FirstOrDefaultAsync();
        //    if (product == null)
        //        throw new Exception($"Product record having Id={productId} not found.");
        //    string xmlFilename = ComposeConentPath(product.SchemaFile);
        //    if (File.Exists(xmlFilename) == false)
        //        throw new Exception($"Product XML schema file not found: {xmlFilename}");

        //    var reader = XmlReader.Create(xmlFilename, new XmlReaderSettings() { Async = true });
        //    var clientProduct = new ClientProduct();
        //    await clientProduct.ImportXmlAsync(reader);

        //    // Verify that the ProductId in the database and XML match.
        //    if (product.Id != clientProduct.ProductId)
        //        throw new Exception($"ProductId(s) do not match (Database:{product.Id} File:{clientProduct.ProductId} Filename:{xmlFilename}).");

        //    clientProduct.Product = product;

        //    if (fullyPopulate == true)
        //    {
        //        foreach (var clientTemplate in clientProduct.Templates)
        //        {
        //            clientTemplate.SiteTemplate = await _db.SiteTemplates.FirstOrDefaultAsync(x => x.Id == clientTemplate.SiteTemplateId);
        //            if (clientTemplate.SiteTemplate == null) throw new Exception($"Template record having Id={clientTemplate.SiteTemplateId} not found.");
        //            foreach (var clientTheme in clientTemplate.Themes)
        //            {
        //                clientTheme.Theme = await _db.Themes.FirstOrDefaultAsync(x => x.Id == clientTheme.ThemeId);
        //                if (clientTheme.Theme == null) throw new Exception($"Theme record having Id={clientTheme.ThemeId} not found.");
        //            }
        //            foreach (var clientModule in clientTemplate.Modules)
        //            {
        //                clientModule.Module = await _db.Modules.FirstOrDefaultAsync(x => x.Id == clientModule.ModuleId);
        //                if (clientModule.Module == null) throw new Exception($"Module record having Id={clientModule.ModuleId} not found.");
        //            }
        //        }
        //    }

        //    return clientProduct;
        //}

        /// <summary>
        /// Creates an instance of ClientProduct and paritially populates it with the XML schema.
        /// note: Only the ids of the Templates, Themes, and Modules will be loaded.
        /// </summary>
        //public async Task<ClientProduct> CreateClientProductAsync(string productId)
        //{
        //    return await InternalCreateClientProductAsync(productId, fullyPopulate: false);
        //}

        /// <summary>
        /// Creates an instance of ClientProduct and fully populates it with the XML schema.
        /// </summary>
        //public async Task<ClientProduct> CreateClientProductDetailsAsync(string productId)
        //{
        //    return await InternalCreateClientProductAsync(productId, fullyPopulate: true);
        //}

        /// <summary>
        /// Re-populate the specified ClientProduct from the assigned product XML and re-enable it.
        /// </summary>
        /// <param name="clientProductId">The Id of ClientProject to be reset.</param>
        /// <returns></returns>



        /// <summary>
        /// Marks the specified client's product or addon as disabled.
        /// </summary>
        //protected async Task DisableClientProductOnlyAsync(string clientId, string productId)
        //{
        //    var clientProduct = await _db.ClientProducts
        //        .Where(c => c.ClientId == clientId && c.ProductId == productId).
        //        FirstOrDefaultAsync();
        //    if (clientProduct != null)
        //    {
        //        clientProduct.SubscriptionEndUTC = DateTime.UtcNow;
        //        await _db.SaveChangesAsync();
        //    }
        //}

        /// <summary>
        /// Marks all of the specified client's products and addons as disabled.
        /// </summary>
        /// <param name="excludeProductId">The ClientProduct with this ProductId will not be disabled.</param>
        /// <returns></returns>
        //protected async Task DisableClientProductCatagoryAsync(string clientId, string catagoryId, string excludeProductId)
        //{
        //    var clientProducts = await _db.ClientProducts
        //        .Include(x => x.Product)
        //        .Where(c => c.ClientId == clientId && c.Product.CategoryId == catagoryId && c.IsActive)
        //        .ToListAsync();

        //    if (clientProducts != null)
        //    {
        //        foreach (var product in clientProducts)
        //            if (product.ProductId.Equals(excludeProductId, StringComparison.OrdinalIgnoreCase) == false)
        //            {
        //                product.SubscriptionEndUTC = DateTime.UtcNow;
        //            }
        //        await _db.SaveChangesAsync();
        //    }
        //}

        /// <summary>
        /// Delete the specified product from the specified client.
        /// Notes: 
        /// The acutal product records will not be deleted. Only the records linking the client to the product will be deleted.
        /// Only the specified client product will be removed.  No associated add-on products will be removed
        /// unless it has the productId that was specified.
        /// </summary>
        //protected async Task RemoveClientProductOnlyAsync(string clientId, string productId)
        //{
        //    //var clientProduct = await _db.ClientProducts
        //    //    .Include(c => c.Settings)
        //    //    .Include(c => c.Templates).ThenInclude(t => t.Themes)
        //    //    .Include(c => c.Templates).ThenInclude(t => t.Modules).ThenInclude(m => m.Settings)
        //    //    .Where(c => c.ClientId == clientId && c.ProductId == productId).
        //        //FirstOrDefaultAsync();
        //    var clientProduct = await _db.ClientProducts
        //        .Where(c => c.ClientId == clientId && c.ProductId == productId)
        //        .FirstOrDefaultAsync();
        //    if (clientProduct != null)
        //    {
        //        _db.Remove(clientProduct);
        //        await _db.SaveChangesAsync();
        //    }
        //}

        /// <summary>
        /// Delete all of the client products that are of the specified catagory.
        /// Notes: 
        /// This should delete the products and assiciated add-ons because they should all have the same catagory.
        /// A client can only have one products per catagory but can have many add-on products of the same catagory.
        /// </summary>
        //protected async Task RemoveClientProductCatagoryAsync(string clientId, string catagoryId)
        //{
        //    var clientProduct = await _db.ClientProducts
        //        //.Include(c => c.Settings)
        //        //.Include(c => c.Templates).ThenInclude(t => t.Themes)
        //        //.Include(c => c.Templates).ThenInclude(t => t.Modules).ThenInclude(m => m.Settings)
        //        .Where(c => c.ClientId == clientId && c.Product.CategoryId == catagoryId).
        //        FirstOrDefaultAsync();
        //    if (clientProduct != null)
        //    {
        //        _db.Remove(clientProduct);
        //        await _db.SaveChangesAsync();
        //    }
        //}

        /// <summary>
        /// Disables the specified client product/addon.
        /// Note: Any addons associated with the specified product will also be disabled.
        /// </summary>
        //public async Task DisableClientProductAsync(string clientId, string productId)
        //{
        //    Ensure.NotNull(clientId);
        //    Ensure.NotNull(productId);

        //    var clientProduct = await _db.ClientProducts
        //        .Include(c => c.Product)
        //        .Where(c => c.ClientId == clientId && c.ProductId == productId)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync();
        //    if ((clientProduct != null) && (clientProduct.Product != null))
        //    {
        //       if (clientProduct.Product.Type == ProductType.Product)
        //            await DisableClientProductCatagoryAsync(clientId, clientProduct.Product.CategoryId, excludeProductId: string.Empty);
        //       else
        //            await DisableClientProductOnlyAsync(clientId, productId);
        //    }
        //}

        //public async Task<IEnumerable<Client>> GetClientProductsAsync()
        //{
        //    return await _db.Clients.AsNoTracking()
        //        .Include(x => x.Products)
        //        .ThenInclude(x => x.Product)
        //        .OrderBy(x => x.Name)
        //        .ToArrayAsync();
        //}

        //public async Task<string> GetClientProductXmlAsync(int clientProductId)
        //{
        //    Ensure.Argument.NotNull(clientProductId, "clientProductId");

        //    var model = _db.ClientProducts.AsNoTracking()
        //        .Include(x => x.Settings)
        //        .Include(x => x.Templates)
        //        .ThenInclude(x => x.Themes)
        //        .Include(x => x.Templates)
        //        .ThenInclude(x => x.Modules)
        //        .ThenInclude(x => x.Settings)
        //        .First(x => x.Id == clientProductId);

        //    var xml = new XDocument();
        //    var stream = new System.IO.MemoryStream();
        //    string content = "";

        //    var productRoot = new XElement("product", new XAttribute("id", model.ProductId));
        //    var templateNodes = new XElement("templates");

        //    productRoot.Add(templateNodes);
        //    productRoot.Add(new XElement("settings", model.Settings.Select(
        //        x => new XElement("setting", new XAttribute[] {
        //            new XAttribute("name", x.FieldName),
        //            new XAttribute("value", x.Value)
        //        })
        //    )));

        //    foreach(var template in model.Templates)
        //    {
        //        var templateNode = new XElement("template", new XAttribute("id", template.SiteTemplateId));
        //        var moduleNodes = new XElement("modules");

        //        templateNodes.Add(templateNode);
        //        templateNode.Add(moduleNodes);

        //        foreach (var module in template.Modules)
        //        {
        //            var moduleNode = new XElement("module", new XAttribute("id", module.ModuleId));
        //            moduleNode.Add(new XElement("settings", module.Settings.Select(
        //                x => new XElement("setting", new XAttribute[] {
        //                    new XAttribute("name", x.FieldName),
        //                    new XAttribute("value", x.Value)
        //                })
        //            )));
        //            moduleNodes.Add(moduleNode);
        //        }

        //        templateNode.Add(new XElement("themes", template.Themes.Select(
        //            x => new XElement("theme", new XAttribute("id", x.ThemeId))
        //        )));                
        //    }

        //    xml.Add(productRoot);
        //    xml.Save(stream);

        //    content = System.Text.UTF8Encoding.UTF8.GetString(stream.ToArray());
        //    stream.Dispose();

        //    return await Task.FromResult(content);
        //}
    }
}
