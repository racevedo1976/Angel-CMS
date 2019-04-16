using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Angelo.Identity.Abstractions
{
    public interface ILdapUser
    {
        
        // Mandatory
        string SubjectId { get; set; }
        string Username { get; set; }
        string ProviderSubjectId { get; set; }
        string ProviderName { get; set; }
        string Email { get; set; }
        bool IsActive { get; set; }
        string DisplayName { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        Guid LdapGuid { get; set; }
        ICollection<Claim> Claims { get; set; }
        ICollection<string> Groups { get; set; }
        string DirectoryId { get; set; }
        /// <summary>
        /// Define the Ldap attributes that will be map on the user.
        /// </summary>
        string[] Attributes { get; }
        string Password { get; set; }

        /// <summary>
        /// Fill the user claims based on the ldapEntry
        /// </summary>
        /// <param name="ldapEntry">The LDAP entry.</param>
        void FillClaims(LdapEntry ldapEntry);

        /// <summary>
        /// Fill the user groups (memberOf) based on the ldapEntry
        /// </summary>
        /// <param name="ldapEntry">The LDAP entry.</param>
        void FillGroups(LdapEntry user);

        /// <summary>
        /// This will set the base details such as:
        /// - DisplayName
        /// - Username
        /// - ProviderName
        /// - SubjectId
        /// - ProviderSubjectId
        /// - Fill the claims
        /// </summary>
        /// <param name="ldapEntry">Ldap Entry</param>
        /// <param name="providerName">Specific provider such as Google, Facebook, etc.</param>
        void SetBaseDetails(LdapEntry ldapEntry, string providerName);

    }
}
