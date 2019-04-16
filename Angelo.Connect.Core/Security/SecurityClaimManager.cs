using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Security
{
    public class SecurityClaimManager
    {
        private PermissionProviderFactory _permissionFactory;
        private Identity.SecurityPoolManager _poolManager;

        public SecurityClaimManager(PermissionProviderFactory permissionFactory, Identity.SecurityPoolManager poolManager)
        {
            _permissionFactory = permissionFactory;
            _poolManager = poolManager;
        }


        public IEnumerable<Permission> GetAllPermissionForPoolId(string poolId)
        {
            var poolType = _poolManager.GetByIdAsync(poolId).Result.PoolType;

            return _permissionFactory.GetPermissions(poolType);
        }
    }
}
