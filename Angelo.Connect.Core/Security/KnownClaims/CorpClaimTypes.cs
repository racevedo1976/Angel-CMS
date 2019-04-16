using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security.KnownClaims
{
    public class CorpClaimTypes
    {
        public const string CorpPrimaryAdmin = "corp-primary-admin";

        public const string CorpCustomersCreate = "corp-customers-create";
        public const string CorpCustomersRead = "corp-customers-read";
        public const string CorpCustomersEdit = "corp-customers-edit";
        public const string CorpCustomersDelete = "corp-customers-delete";

        public const string CorpSitesCreate = "corp-sites-create";
        public const string CorpSitesRead = "corp-sites-read";
        public const string CorpSitesEdit = "corp-sites-edit";
        public const string CorpSitesDelete = "corp-sites-delete";

        public const string CorpUserDirectoryCreate = "corp-dir-create";
        public const string CorpUserDirectoryRead = "corp-dir-read";
        public const string CorpUserDirectoryEdit = "corp-dir-edit";
        public const string CorpUserDirectoryDelete = "corp-dir-delete";

        public const string CorpProductsCreate = "corp-products-create";
        public const string CorpProductsRead = "corp-products-read";
        public const string CorpProductsEdit = "corp-products-edit";
        public const string CorpProductsDelete = "corp-products-delete";
        public const string CorpProductsAssign = "corp-products-map";

        public const string CorpUser = "corp-user";

        public const string CorpUsersCreate = "corp-users-create";
        public const string CorpUsersRead = "corp-users-read";
        public const string CorpUsersEdit = "corp-users-edit";
        public const string CorpUsersDelete = "corp-users-delete";

        public const string CorpRolesCreate = "corp-roles-create";
        public const string CorpRolesRead = "corp-roles-read";
        public const string CorpRolesEdit = "corp-roles-edit";
        public const string CorpRolesDelete = "corp-roles-delete";
        public const string CorpRolesAssign = "corp-roles-map";

        public const string CorpLibraryBrowse = "corp-library-read";
        public const string CorpLibraryCreateFolders = "corp-library-folders-create";
        public const string CorpLibraryEditFolders = "corp-library-folders-edit";
        public const string CorpLibraryDeleteFolders = "corp-library-folders-delete";
        public const string CorpLibraryUploadFiles = "corp-library-files-upload";
        public const string CorpLibraryEditFiles = "corp-library-files-edit";
        public const string CorpLibraryDeleteFiles = "corp-library-files-delete";
        public const string CorpLibraryManageSecurity = "corp-library-security";

        public const string CorpJobsRead = "corp-jobs-read";
        public const string CorpJobsExecute = "cop-jobs-run";

    }
}
