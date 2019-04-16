using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Common.Extensions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A24205_SeedInternalProduct: IAppMigration
    {
        public string Id { get; } = "A24205";

        public string Migration { get; } = "Seed Internal Product";
     
        private ConnectDbContext _connectDb;

        public A24205_SeedInternalProduct(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            // Fail if cannot connect to db
            if(_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Check if internal product already exists
            var exists = await _connectDb.Products.AnyAsync(x => x.Id == DbKeys.ProductIds.Internal);

            if (exists)
            {
                // early terminate
                return MigrationResult.Skipped("Product already exists");
            }

            // Register the product
            var internalProduct = new Product
            {
                Id = DbKeys.ProductIds.Internal,
                CategoryId = DbKeys.ProductCategoryIds.Angelo,
                Name = "Internal Corp Product",
                Description = "Internal product used for building PC|Mac demo sites.",
                SchemaFile = "/schemas/products/Product-0-Internal.json",
                Active = true
            };

            _connectDb.Products.Add(internalProduct);
            await _connectDb.SaveChangesAsync();


            // Update the PcMac Product Mapping
            var productMapping = await _connectDb.ClientProductApps
                .Include(x => x.AddOns)
                .FirstOrDefaultAsync(
                    x => x.Id == DbKeys.ClientProductAppIds.PcMacApp1
                );
      
            productMapping.ProductId = DbKeys.ProductIds.Internal;
            productMapping.Title = internalProduct.Name;
            productMapping.MaxSiteCount = 500; // was previously 5

            // Remove any previously seeded add-ons (the new product doesn't support any)
            if (productMapping.AddOns != null)
            {
                (productMapping.AddOns as List<ClientProductAddOn>).RemoveAll(x => true);
            }

            // Done
            await _connectDb.SaveChangesAsync();

            return MigrationResult.Success("Created product and mapped to PcMac client.");         
        }
    }
}
