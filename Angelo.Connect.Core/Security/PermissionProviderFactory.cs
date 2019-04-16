using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Identity.Models;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security
{
    public class PermissionProviderFactory
    {
        private IEnumerable<ISecurityPermissionProvider> _permissionProviders;

        public PermissionProviderFactory(IEnumerable<ISecurityPermissionProvider> permissionProviders)
        {
            _permissionProviders = permissionProviders;
        }

        public IEnumerable<Permission> GetPermissions(PoolType level)
        {
            var permissions = new List<Permission>();

            foreach(var provider in _permissionProviders)
            {
                if(provider.Level == level)
                {
                    permissions.AddRange(provider.PermissionGroups());
                }
            }

            if(permissions.Count == 0)
                throw new NotImplementedException($"No permissions registered for level {level.ToString()}");

            return permissions;
        }
    }
}
