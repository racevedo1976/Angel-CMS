using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Identity.Models;


namespace Angelo.Identity
{
    public class TenantManager
    {
        private IdentityDbContext _identityDb;

        public TenantManager(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<Tenant> GetByIdAsync(string tenantId)
        {
            Ensure.Argument.NotNull(tenantId);

            return await _identityDb.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId);
        }

        public async Task<Tenant> GetByKeyAsync(string tenantKey)
        {
            Ensure.Argument.NotNull(tenantKey);

            return await _identityDb.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);
        }

        public async Task<IEnumerable<TenantUri>> GetUrisAsync(string tenantKey)
        {
            Ensure.Argument.NotNull(tenantKey);

            return await _identityDb.TenantUris
                .Where(x => x.Tenant.Key == tenantKey)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUrisAsync(string tenantKey, TenantUriType uriType)
        {
            Ensure.Argument.NotNull(tenantKey);
            Ensure.Argument.NotNull(uriType);

            return await _identityDb.TenantUris
                .Where(x => x.Tenant.Key == tenantKey && x.Type == uriType)
                .Select(x => x.Uri)
                .ToListAsync();
        }

        public async Task<Tenant> CreateAsync(string tenantKey, string tenantName)
        {
            if (await KeyExistsAsync(tenantKey))
                throw new Exception($"Cannot create tenant {tenantName}. Tenant key {tenantKey} already exists.");

            var tenant = new Tenant()
            {
                Id = KeyGen.NewGuid(),
                Key = tenantKey,
                Name = tenantName,
                OidcTitle = tenantName
            };

            _identityDb.Tenants.Add(tenant);
            await _identityDb.SaveChangesAsync();

            return tenant;
        }
  
        public async Task UpdateKeyAsync(string tenantKey, string newTenantKey)
        {
            Ensure.Argument.NotNull(tenantKey);
            Ensure.Argument.NotNull(newTenantKey);

            // early exist if exact same
            if (tenantKey == newTenantKey)
                return;

            // Check to see if the key is already in use 
            // Skip if we're just changing the case to prevent false positive
            if(tenantKey.ToLower() != newTenantKey.ToLower())
            {
                if (await KeyExistsAsync(newTenantKey))
                    throw new Exception($"Cannot change tenant key. Tenant key {newTenantKey} is already in use.");
            }

            // Update the tenant
            var tenant = await _identityDb.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);

            if (tenant == null)
                throw new NullReferenceException($"Cannot change tenant key. Tenant {tenantKey} does not exist.");

            tenant.Key = newTenantKey;

            await _identityDb.SaveChangesAsync();
        }

        public async Task<bool> KeyExistsAsync(string tenantKey)
        {
            return await _identityDb.Tenants.AnyAsync(x => x.Key == tenantKey);
        }

        public async Task AddUriAsync(string tenantKey, string uri, TenantUriType uriType)
        {
            var tenant = await _identityDb.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);

            if (tenant == null)
                throw new NullReferenceException($"Tenant {tenantKey} does not exist.");

            uri = NormalizeTenantUri(uri);

            var exists = await _identityDb.TenantUris
                    .AnyAsync(x => x.TenantId == tenantKey && x.Uri == uri && x.Type == uriType);

            if (!exists)
            {
                _identityDb.TenantUris.Add(new TenantUri
                {
                    Id = KeyGen.NewGuid(),
                    TenantId = tenant.Id,
                    Type = uriType,
                    Uri = uri
                });

                await _identityDb.SaveChangesAsync();
            }

        }

        public async Task RemoveUriAsync(string tenantKey, string uri, TenantUriType uriType)
        {
            var tenant = await _identityDb.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);

            if (tenant == null)
                throw new NullReferenceException($"Tenant {tenantKey} does not exist.");

            uri = NormalizeTenantUri(uri);

            var uriEntry = await _identityDb.TenantUris.FirstOrDefaultAsync(x =>
                x.TenantId == tenant.Id
                && x.Type == uriType
                && x.Uri == uri
            );

            if (uriEntry != null)
            {
                _identityDb.TenantUris.Remove(uriEntry);
                await _identityDb.SaveChangesAsync();
            }
        }

        public async Task<int> RemoveUrisMatchingAsync(string tenantKey, string partialUri)
        {
            var tenant = await _identityDb.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);

            if (tenant == null)
                throw new NullReferenceException($"Tenant {tenantKey} does not exist.");

            partialUri = NormalizeTenantUri(partialUri);

            var matchingUris = await _identityDb.TenantUris
                .Where(x =>
                    x.TenantId == tenantKey
                    && x.Uri.StartsWith(partialUri, StringComparison.OrdinalIgnoreCase)
                )
                .ToListAsync();

            _identityDb.TenantUris.RemoveRange(matchingUris);

            await _identityDb.SaveChangesAsync();

            return matchingUris.Count;
        }
        
        private string NormalizeTenantUri(string uri)
        {
            uri = uri.Replace("https://", "");
            uri = uri.Replace("http://", "");

            if (uri.EndsWith("/"))
                uri = uri.Substring(0, uri.Length - 1);

            return uri.ToLower();
        }

    }
}
