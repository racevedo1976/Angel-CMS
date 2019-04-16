using Angelo.Identity.Models;
using System.Collections.Generic;

namespace Angelo.Identity.Abstractions
{
    /// <summary>
    /// Maybe not mandatory, to see.
    /// </summary>
    public interface ILdapService<out TUser>
        where TUser : ILdapUser, new()
    {
        /// <summary>
        /// Logins using the specified credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Returns the logged in user.</returns>
        TUser Login(string username, string password);

        /// <summary>
        /// Finds user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>Returns the user when it exists.</returns>
        TUser FindUser(string username);

        /// <summary>
        /// Finds users by filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Returns the users.</returns>
        List<ILdapUser> SearchUsers(string filter);
        List<ILdapUser> SearchUsersPaged(string filter);

        
        LdapDomain LdapConfig { set; }
    }
}
