using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Security.KnownClaims;
using Angelo.Identity.Models;

namespace Angelo.Connect.Security
{
    public class PermissionProviderCorpLevel : ISecurityPermissionProvider
    {
        public PoolType Level { get; } = PoolType.Corporate;

        private const string _corpId = "MyCompany";
        private List<Permission> _permissions;

        public PermissionProviderCorpLevel()
        {
            _permissions = new List<Permission>();

            CreatePermissions();          
        }


        private void CreatePermissions()
        {
            _permissions.Add(new Permission
            {
                Title = "Users & Security",
                Permissions = new Permission[]
                {
                    new Permission {
                        Title = "Browse Corp User Directories",
                        Claims = {
                            new SecurityClaim(CorpClaimTypes.CorpUserDirectoryRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpUsersRead, _corpId),
                        }
                    },
                    new Permission {
                        Title = "Manage / Create Corp Directories",
                        Claims = {
                            new SecurityClaim(CorpClaimTypes.CorpUserDirectoryCreate, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpUserDirectoryRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpUserDirectoryEdit, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpUserDirectoryDelete, _corpId),
                        }
                    },
                    new Permission {
                        Title = "Create / Edit Corp Users",
                        Claims = {
                            new SecurityClaim(CorpClaimTypes.CorpUserDirectoryRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpUsersRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpUsersCreate, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpUsersEdit, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpUsersDelete, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpRolesAssign, _corpId),
                        }
                    },
                    new Permission {
                        Title = "Create / Edit Corp Roles",
                        Claims = {
                            new SecurityClaim(CorpClaimTypes.CorpRolesRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpRolesCreate, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpRolesEdit, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpRolesDelete, _corpId),
                        }
                    },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Customers & Products",
                Permissions = new Permission[]
                {
                    new Permission
                    {
                        Title = "Manage / Create Customers",
                        Claims =
                        {
                            new SecurityClaim(CorpClaimTypes.CorpCustomersCreate, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpCustomersRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpCustomersEdit, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpCustomersDelete, _corpId),
                        }
                    },
                    new Permission
                    {
                        Title = "Edit Product Definitions",
                        Claims =
                        {
                            new SecurityClaim(CorpClaimTypes.CorpProductsCreate, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpProductsRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpProductsEdit, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpProductsDelete, _corpId),
                        }
                    },
                    new Permission
                    {
                        Title = "Assign Products to Customers",
                        Claims =
                        {
                            new SecurityClaim(CorpClaimTypes.CorpCustomersRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpProductsRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpProductsAssign, _corpId),
                        }
                    },
                    new Permission
                    {
                        Title = "Impersonate & Support Customers",
                        Claims =
                        {
                            new SecurityClaim(CorpClaimTypes.CorpProductsRead, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpCustomersRead, _corpId),
                            new SecurityClaim(ClientClaimTypes.PrimaryAdmin, _corpId),
                            new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _corpId),
                        }
                    }
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Corporate Library",
                Permissions =
                {
                    new Permission {
                        Title = "Browse Corp Document Library",
                        Claims =
                        {
                            new SecurityClaim(CorpClaimTypes.CorpLibraryBrowse, _corpId),
                        }
                    },
                    new Permission {
                        Title = "Manage Corp Document Library",
                        Claims =
                        {
                            new SecurityClaim(CorpClaimTypes.CorpLibraryBrowse, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpLibraryCreateFolders, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpLibraryEditFolders, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpLibraryDeleteFolders, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpLibraryUploadFiles, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpLibraryEditFiles, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpLibraryDeleteFiles, _corpId),
                            new SecurityClaim(CorpClaimTypes.CorpLibraryManageSecurity, _corpId),
                        }
                    },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "All Client Libraries",
                Permissions =
                {
                    new Permission {
                        Title = "Browse All Clients Document Library",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteLibraryReader, _corpId),
                        }
                    },
                    new Permission {
                        Title = "Manage All Clients Document Libraries",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteLibraryOwner, _corpId),
                            new SecurityClaim(SiteClaimTypes.SiteLibraryReader, _corpId)
                        }
                    },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Server Jobs",
                Permissions =
                {
                    new Permission {
                        Title = "View Job Status",
                        Claims =
                        {
                            new SecurityClaim(CorpClaimTypes.CorpJobsRead, _corpId),
                        }
                    },
                    new Permission {
                        Title = "Execute Jobs",
                        Claims =
                        {
                            new SecurityClaim(CorpClaimTypes.CorpJobsExecute, _corpId),
                        }
                    },
                }
            });

        }


        public IEnumerable<Permission> PermissionGroups()
        {
            return _permissions;
        }
    }
}
