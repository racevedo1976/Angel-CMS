using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Serialization;
using Novell.Directory.Ldap;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Identity;
using Angelo.Identity.Models;
using Angelo.Identity.Services;

namespace Angelo.Connect.Web.UI.Controllers.Api.CorpLevel
{
    public class CorpLdapDataController : Controller
    {
        private ClientManager _clientManager;
        DirectoryManager _directoryManager;
        SecurityPoolManager _poolManager;
        RoleManager _roleManager;
        LdapManager _ldapManager;

        public CorpLdapDataController(ClientManager clientManager, DirectoryManager directoryManager, SecurityPoolManager poolManager, RoleManager roleManager, LdapManager ldapManager)
        {
            _clientManager = clientManager;
            _directoryManager = directoryManager;
            _poolManager = poolManager;
            _roleManager = roleManager;
            _ldapManager = ldapManager;
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpGet, Route("/sys/corp/api/ldap/hierarchy")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> FetchHierarchy([DataSourceRequest]DataSourceRequest request, string id, string clientId)
        {
            object model = null;


            if (string.IsNullOrEmpty(id))
            {
                // client has been switched or initial request to the page
                var client = await _clientManager.GetByIdAsync(clientId);

                // HOTFIX: Directory manager requires tenantKey, not clientId
                var directories = await _directoryManager.GetDirectoriesWithMapAsync(client.TenantKey);

                model = directories.Select(x => new
                {
                    id = $"{typeof(Directory).Name.ToString()}_{x.Id}",   // id = [Directory]_[directoryId]
                    Name = x.Name,
                    hasChildren = x.DirectoryMap.Any()
                });
            }
            else
            {

                // parse the tree node's string id to determine action
                var nodeInfo = id.Split('_');
                var nodeType = nodeInfo[0];
                
                switch (nodeType)
                {

                    case "Directory":

                        var directoryId = nodeInfo[1];
                        var poolMaps = await _directoryManager.GetDirectoryPoolsAsync(directoryId);
                        var pools = new List<Identity.Models.SecurityPool>();
                        
                        foreach (var pool in poolMaps)
                        {
                            var poolObject = await _poolManager.GetByIdAsync(pool.PoolId);
                            pools.Add(poolObject);
                        }

                        model = pools.Select(x => new
                        {
                            id = $"{typeof(Identity.Models.SecurityPool).Name.ToString()}_{x.PoolId}_{directoryId}",  // id = [secPool]_[poolId]_[directoryId]
                            Name = x.Name,
                            hasChildren = (_poolManager.GetRolesQuery(x.PoolId)).Any()
                        });
                        break;

                    case "SecurityPool":
                        var poolId = nodeInfo[1];
                        var roles = await _poolManager.GetRolesAsync(poolId);

                        directoryId = nodeInfo[2];

                        model = roles.Select(x => new
                        {
                            id = $"{typeof(Role).Name.ToString()}_{x.Id}_{poolId}_{directoryId}",  // id = [role]_[roleid]_[poolId]_[directoryId]
                            Name = x.Name,
                            hasChildren = false
                        });
                        break;

                    default:
                        break;
                }
            }

            return new JsonResult(model, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            // return Json(model.ToDataSourceResult(request));
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpGet, Route("/sys/corp/api/ldap")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetDirectories(string id , string directoryId, string poolId)
        {
            Ensure.NotNullOrEmpty(directoryId, "Directory is required");
            var filterGroup = string.Empty;
            var ldapDomain = await _directoryManager.GetDirectoryLdapAsync(directoryId);

            if (!string.IsNullOrEmpty(poolId)){
                var pool = await _poolManager.GetByIdAsync(poolId);
                if (pool != null)
                {
                    filterGroup = pool.LdapFilterGroup;
                }
            };

            //resolve the baseDn to start search directory
            //id will always be empty on opening the ldap browser, since no nodes are selected
            //check if pool has a default group selection
            //if not then default to dir root.
            var baseDn= string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                baseDn = id;
            }else if (!string.IsNullOrEmpty(filterGroup))
            {
                baseDn = filterGroup;
            }
            else
            {
                baseDn = ldapDomain.LdapBaseDn;
            }

            //Get directory contents (nodesObjects)
            var entries = _ldapManager.GetDirectoryEntries(ldapDomain, baseDn);

            return new JsonResult(entries, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpGet, Route("/sys/corp/api/ldap/test")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> LdapTestConnection(LdapDomain ldapDomain)
        {
            var connection = new LdapConnection();
            var message = string.Empty;
            var result = false;
            try
            {
                //Get directory contents (nodesObjects)
                connection = (LdapConnection)_ldapManager.GetConnection(ldapDomain);
                result = connection.Connected;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return new JsonResult(new { connected = result, message = message }, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpGet, Route("/sys/corp/api/ldap/entry/attr")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetEntryNodeAttributes(string searchDn, string directoryId)
        {

            var ldapDomain = await _directoryManager.GetDirectoryLdapAsync(directoryId);
            
            //Get directory contents (nodesObjects)
            var attributes = _ldapManager.GetEntryAttributes(ldapDomain, searchDn);

            return new JsonResult(attributes, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpPost, Route("/sys/corp/api/ldap/save")]
        public async Task<IActionResult> SaveLdapSetting(LdapDomain ldap)
        {
            if (!string.IsNullOrEmpty(ldap.Id))
            {
                var savedLdap = await _directoryManager.GetDirectoryLdapAsync(ldap.DirectoryId);
                savedLdap.Host = ldap.Host;
                savedLdap.Domain = ldap.Domain;
                savedLdap.User = ldap.User;
                savedLdap.Password = ldap.Password;
                savedLdap.LdapBaseDn = ldap.LdapBaseDn;
                savedLdap.UseSsl = ldap.UseSsl;

                _directoryManager.UpdateDirectoryLdapAsync(savedLdap);
            }
            else
            {
                _directoryManager.SaveDirectoryLdapAsync(ldap);
            }

            return Ok(ldap);
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpPost, Route("/sys/corp/api/ldaprolemapping/save")]
        public async Task<IActionResult> SaveLdapMapping(LdapMapping[] ldapMappings, string roleId)
        {
            if ((ldapMappings.Count() != 0))
            {
                await _roleManager.DeleteLdapMappingAsync(roleId);

                foreach (var map in ldapMappings)
                {

                    await _roleManager.AddLdapMappingAsync(map);
                }

               
            }
           

            return Ok(ldapMappings);
        }

        [Authorize(policy: PolicyNames.CorpUser)]
        [HttpPost, Route("/sys/corp/api/ldapsecuritypoolgroup/save")]
        public async Task<IActionResult> SaveLdapGroupFilter(Identity.Models.SecurityPool ldapSec)
        {
            if ((ldapSec.PoolId != null))
            {
                var savedSec = await _poolManager.GetByIdAsync(ldapSec.PoolId);
                savedSec.LdapFilterGroup = ldapSec.LdapFilterGroup;

                await _poolManager.UpdateAsync(savedSec);
            }
           

            return Ok(ldapSec);
        }


    }
}
