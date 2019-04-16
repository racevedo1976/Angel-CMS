using Angelo.Identity.Abstractions;
using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;
using System.Security.Claims;

namespace Angelo.Identity.Models
{
    public class LdapUser : ILdapUser
    {

        private string _subjectId = null;

        public string SubjectId
        {
            get { return _subjectId ?? Username; }
            set { _subjectId = value; }
            }

        public string ProviderSubjectId { get; set; }
        public string ProviderName { get; set; }

        public string DisplayName { get; set; }
        public string Username { get; set; }

        public bool IsActive
        {
            get { return true; } // Always true for us, but we should look if the account have been locked out.
            set { }
        }

        public ICollection<Claim> Claims { get; set; }
        public ICollection<string> Groups { get; set; }
        public string[] Attributes { get; set; } // => LdapAttributesExtensions.ToDescriptionArray<LdapAttributes>.Descriptions; 
        public string Email { get; set; }
        public Guid LdapGuid { get; set; }
        public string DirectoryId { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        /// <summary>
        /// Fills the claims.
        /// </summary>
        /// <param name="user">The user.</param>
        public void FillClaims(LdapEntry user)
        {
            //// Example in LDAP we have display name as displayName (normal field)
            //this.Claims = new List<Claim>
            //    {
            //        GetClaimFromLdapAttributes(user, JwtClaimTypes.Name, LdapAttributes.DisplayName),
            //        GetClaimFromLdapAttributes(user, JwtClaimTypes.FamilyName, LdapAttributes.LastName),
            //        GetClaimFromLdapAttributes(user, JwtClaimTypes.GivenName, LdapAttributes.FirstName),
            //        GetClaimFromLdapAttributes(user, JwtClaimTypes.Email, LdapAttributes.EMail),
            //        GetClaimFromLdapAttributes(user, JwtClaimTypes.PhoneNumber, LdapAttributes.TelephoneNumber)
            //    };

            //// Add claims based on the user groups
            //// add the groups as claims -- be careful if the number of groups is too large
            //if (true)
            //{
            //    try
            //    {
            //        var userRoles = user.getAttribute(LdapAttributes.MemberOf.ToDescriptionString()).StringValues;
            //        while (userRoles.MoveNext())
            //        {
            //            this.Claims.Add(new Claim(JwtClaimTypes.Role, userRoles.Current.ToString()));
            //        }
            //        //var roles = userRoles.Current (x => new Claim(JwtClaimTypes.Role, x.Value));
            //        //id.AddClaims(roles);
            //        //Claims = this.Claims.Concat(new List<Claim>()).ToList();
            //    }
            //    catch (Exception)
            //    {
            //        // No roles exists it seems.
            //    }
            //}
        }

        public void FillGroups(LdapEntry user)
        {
            this.Groups = new List<string>();
            try
            {
                var userRoles = user.getAttribute(LdapAttributes.MemberOf.ToDescriptionString()).StringValues;
                while (userRoles.MoveNext())
                {
                    this.Groups.Add(userRoles.Current.ToString());
                }
                //var roles = userRoles.Current (x => new Claim(JwtClaimTypes.Role, x.Value));
                //id.AddClaims(roles);
                //Claims = this.Claims.Concat(new List<Claim>()).ToList();
            }
            catch (Exception ex)
            {
                // No roles exists it seems.
            }
        }

        public static string[] RequestedLdapAttributes()
        {
            throw new NotImplementedException();
        }

        internal Claim GetClaimFromLdapAttributes(LdapEntry user, string claim, LdapAttributes ldapAttribute)
        {
            string value = string.Empty;

            try
            {
                value = user.getAttribute(ldapAttribute.ToDescriptionString()).StringValue;
                return new Claim(claim, value);
            }
            catch (Exception)
            {
                // Should do something... But basically the attribute is not found
                // We swallow for now, since we might not care.
            }

            return new Claim(claim, value);
        }
       
        public void SetBaseDetails(LdapEntry ldapEntry, string providerName)
        {
            DisplayName = ldapEntry.getAttribute(LdapAttributes.DisplayName.ToDescriptionString())?.StringValue;
            Username = ldapEntry.getAttribute(LdapAttributes.sAMAccountName.ToDescriptionString())?.StringValue;
            FirstName = ldapEntry.getAttribute(LdapAttributes.FirstName.ToDescriptionString())?.StringValue;
            LastName = ldapEntry.getAttribute(LdapAttributes.LastName.ToDescriptionString())?.StringValue;
            Email = ldapEntry.getAttribute(LdapAttributes.EMail.ToDescriptionString())?.StringValue ?? ldapEntry.getAttribute(LdapAttributes.EMailFromPrincipal.ToDescriptionString())?.StringValue;
            LdapGuid = new Guid((Byte[])(Array)ldapEntry.getAttribute(LdapAttributes.LdapGuid.ToDescriptionString())?.ByteValue);
            ProviderName = providerName;
            SubjectId = Username; // Extra: We could use the uidNumber instead in a sha algo.
            ProviderSubjectId = Username;
            FillClaims(ldapEntry);
            FillGroups(ldapEntry);
        }
    }
}
