using Angelo.Identity.Abstractions;
using Angelo.Identity.Models;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Controls;
using System;
using System.Collections.Generic;
using System.Text;


namespace Angelo.Identity.Services
{
    /// <summary>
    /// This is an implementation of the service that is used to contact Ldap.
    /// </summary>
    public class LdapService<TUser> : ILdapService<TUser> where TUser : ILdapUser, new()
    {
        private readonly ILogger<LdapService<TUser>> _logger;
        private LdapDomain _config;
        private LdapConnection _ldapConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="LdapService{TUser}"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public LdapService(
             ILogger<LdapService<TUser>> logger
            )
        {
            _logger = logger;
            _ldapConnection = new LdapConnection() { SecureSocketLayer = false };
           
        }

        public LdapDomain LdapConfig { set { _config = value; } }
        private int LdapPort
        {
            get
            {
                // if (_config.po == 0)
                // {
                return _config.UseSsl ? LdapConnection.DEFAULT_SSL_PORT : LdapConnection.DEFAULT_PORT;
                                                    // }

                //return _config.Port;
            }
        }

        /// <summary>
        /// Logins using the specified credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// Returns the logged in user.
        /// </returns>
        /// <exception cref="LoginFailedException">Login failed.</exception>
        public TUser Login(string username, string password)
        {

            try
            {
                LdapEntry user = null;

                var searchResult = SearchUser(username);

                if (searchResult.hasMore())
                {
                    try
                    {
                        user = searchResult.next();
                        if (user != null)
                        {
                            _logger.LogDebug($"[LdapService] => User found. User: {username}");
                            _logger.LogDebug($"[LdapService] => User {username}. Member of: {user.getAttribute("MEMBEROF")?.StringValue ?? ""}");
                            _logger.LogDebug($"[LdapService] => User {username}. Ldap User Settings: {ldapUserAccountControlValue(user)}. {PasswordStatusValue(user)}");

                            //give priority to Ldap disabled setting.
                            if (IsDisabled(user))
                            {
                                throw new Exception("User is disabled.");
                            }

                            GetConnection();
                            _ldapConnection.Bind(user.DN, password);
                            if (_ldapConnection.Bound)
                            {
                                var appUser = new TUser();
                                appUser.SetBaseDetails(user, "local"); // Should we change to LDAP.
                                
                                _ldapConnection.Disconnect();

                                return appUser;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (user == null) _logger.LogDebug($"[LdapService] => User: {username} not found.");
                        _logger.LogDebug($"[LdapService] => {e.Message}. User: {username}");
                        _logger.LogError($"[LdapService] => {e.Message}. User: {username}");
                        //throw new Exception("Ldap Login failed.", e);
                    }
                }

               
            }
            catch (Exception ex)
            {
                //log the error, perhaps
                _logger.LogTrace($"[LdapService] => {ex.Message}.");
                _logger.LogError($"[LdapService] => {ex.Message}.");
                //throw;
            }
            finally
            {
                _ldapConnection.Disconnect();
            }
           

            return default(TUser);
        }

        private bool IsDisabled(LdapEntry user)
        {
            var acctValue = int.Parse(user.getAttribute("userAccountControl")?.StringValue ?? "0");

            //514 = disabled user
            if (acctValue == 514)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        /// Returns the user when it exists.
        /// </returns>
        public TUser FindUser(string username)
        {
            var searchResult = SearchUser(username);

            try
            {
                var user = searchResult.next();
                if (user != null)
                {
                    var appUser = new TUser();
                    appUser.SetBaseDetails(user, "local");

                    _ldapConnection.Disconnect();

                    return appUser;
                }
            }
            catch (Exception e)
            {
                _logger.LogTrace(e.Message);
                _logger.LogTrace(e.StackTrace);
                _logger.LogError(e.Message);
                // Swallow the exception since we don't expect an error from this method.
            }

            _ldapConnection.Disconnect();

            return default(TUser);
        }

        private LdapSearchResults SearchUser(string username)
        {
            //Bind function with null user dn and password value will perform anonymous bind to LDAP server
            //First figure the user structure 

            GetConnection();

            var attributes = (new TUser()).Attributes;
            var searchFilter = $"(sAMAccountName={username})";
            var baseDn = $"{_config.LdapBaseDn}";

            var result = _ldapConnection.Search(
                baseDn,
                LdapConnection.SCOPE_SUB,
                searchFilter,
                null,
                false
            );

            return result;
        }

        public List<ILdapUser> SearchUsers(string filter)
        {

            
            //Bind function with null user dn and password value will perform anonymous bind to LDAP server
            //First figure the user structure 

            GetConnection();

            var ldapUsers = new List<ILdapUser>();

            var attributes = (new TUser()).Attributes;
            var searchFilter = $"{filter}";
            var baseDn = $"{_config.LdapBaseDn}";

            var results = _ldapConnection.Search(
                baseDn,
                LdapConnection.SCOPE_SUB,
                searchFilter,
                null,
                false
            );
           
            while (results.hasMore())
            {
                try
                {
                    var user = results.next();
                    if (user != null)
                    {

                        var appUser = new TUser();
                        appUser.SetBaseDetails(user, "local"); // Should we change to LDAP.
                        ldapUsers.Add(appUser);
                                                
                    }
                }
                catch (Exception e)
                {
                    _logger.LogTrace(e.Message);
                    _logger.LogTrace(e.StackTrace);
                    _logger.LogError(e.Message);
                    //throw new Exception("Ldap Login failed.", e);
                }
            }

            return ldapUsers;
        }


        public List<ILdapUser> SearchUsersPaged(string filter)
        {

            var ldapUsers = new List<ILdapUser>();

            //Bind function with null user dn and password value will perform anonymous bind to LDAP server
            //First figure the user structure 

            try
            {

                GetConnection();

                // We will be sending two controls to the server 
                LdapSearchConstraints cons = _ldapConnection.SearchConstraints;

                // hardcoded results page size
                int pageSize = 500;
                // initially, cookie must be set to an empty string
                string cookie = "";

                do
                {
                    LdapControl[] requestControls = new LdapControl[1];
                    requestControls[0] = new LdapPagedResultsControl(pageSize, cookie);
                    cons.setControls(requestControls);
                    _ldapConnection.Constraints = cons;

                    // Send the search request - Synchronous Search is being used here 
                    //System.Console.Out.WriteLine("Calling Asynchronous Search...");
                    LdapSearchResults res = _ldapConnection.Search(_config.LdapBaseDn, LdapConnection.SCOPE_SUB, filter, null, false, (LdapSearchConstraints)null);

                    // Loop through the results and print them out
                    while (res.hasMore())
                    {

                        /* 
                         * Get next returned entry.  Note that we should expect a Ldap-
                         * Exception object as well, just in case something goes wrong
                         */
                        LdapEntry user = null;
                        try
                        {
                            user = res.next();

                            if (user != null)
                            {
                                var appUser = new TUser();
                                appUser.SetBaseDetails(user, "local"); // Should we change to LDAP.
                                ldapUsers.Add(appUser);
                                
                            }
                        }
                        catch (LdapException e)
                        {
                            if (e is LdapReferralException)
                                continue;
                            else
                            {
                                //System.Console.Out.WriteLine("Search stopped with exception " + e.ToString());
                                break;
                            }
                        }
                        
                    }

                    // Server should send back a control irrespective of the 
                    // status of the search request
                    LdapControl[] controls = res.ResponseControls;
                    if (controls == null)
                    {
                        Console.Out.WriteLine("No controls returned");
                    }
                    else
                    {
                        // Multiple controls could have been returned
                        foreach (LdapControl control in controls)
                        {
                            /* Is this the LdapPagedResultsResponse control? */
                            if (control is LdapPagedResultsResponse)
                            {
                                LdapPagedResultsResponse response = new LdapPagedResultsResponse(control.ID, control.Critical, control.getValue());

                                cookie = response.Cookie;

                            }
                        }
                    }
                    // if cookie is empty, we are done.
                } while (!String.IsNullOrEmpty(cookie));

                /* We are done - disconnect */
                if (_ldapConnection.Connected)
                    _ldapConnection.Disconnect();
            }
            catch (LdapException e)
            {
                //Console.Out.WriteLine(e.ToString());
                _logger.LogError($"[LdapService] => {e.ToString()}");
            }
            catch (System.IO.IOException e)
            {
                //Console.Out.WriteLine("Error: " + e.ToString());
                _logger.LogError($"[LdapService] => {e.ToString()}");
            }
            catch (Exception e)
            {
                //Console.WriteLine("Error: " + e.Message);
                _logger.LogError($"[LdapService] => {e.Message}");
            }
            
            return ldapUsers;
        }

        public LdapConnection GetConnection()
        {
            
            int port = LdapPort;

            if (!_ldapConnection.Connected)
            {
                try
                {
                    _ldapConnection = new LdapConnection() { SecureSocketLayer = false };

                    //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure    
                    _ldapConnection.Connect(_config.Host, port);
                   
                    //Bind function with null user dn and password value will perform anonymous bind to LDAP server
                    //First figure the user structure 
                    string lpdaUser = "";
                    if (_config.User.Contains(@"\") || (_config.User.Contains("\\")))
                    {
                        lpdaUser = _config.User;
                    }
                    else
                    {
                        lpdaUser = $@"{_config.Domain}\{_config.User}";
                    }

                    _ldapConnection.Bind(lpdaUser, _config.Password);

                    _logger.LogDebug($"[LdapService] =>  Ldap connection success. {_config.Host}.");

                }
                catch (Exception ex)
                {
                    _logger.LogError($"[LdapService] => Error during connection. {ex.Message}. {_config.Host}");
                }

               
            }
            return _ldapConnection;

        }

        private string ldapUserAccountControlValue(LdapEntry ldapUser)
        {

            int flags = int.Parse(ldapUser.getAttribute("userAccountControl")?.StringValue ?? "0");

            string value = "";

            switch (flags)
            {
                default:
                    value = flags.ToString();
                    break;
                case 512:
                    value = "Enabled Account";
                    break;
                case 514:
                    value = "Disabled Account";
                    break;
                case 528:
                    value = "Locked Out";
                    break;
                case 544:
                    value = "Enabled, Password Not Required";
                    break;
                case 546:
                    value = "Disabled, Password Not Required";
                    break;
                case 66048:
                    value = "Enabled, Password Doesn't Expire";
                    break;
                case 66050:
                    value = "Disabled, Password Doesn't Expire";
                    break;
                case 66080:
                    value = "Enabled, Password Doesn't Expire & Not Required";
                    break;
                case 66082:
                    value = "Disabled, Password Doesn't Expire & Not Required";
                    break;
                case 2080:
                    value = "Interdomain Trust Account";
                    break;
                case 4194816:
                    value = "Enabled, This account does not require Kerberos pre-authentication";
                    break;
                case 524800:
                    value = "Enabled, Trusted For Delegation";
                    break;
                case 590336:
                    value = "Enabled, Password Doesn't Expire & Password cannot be changed";
                    break;

            }
            return value;

        }

        private string PasswordStatusValue(LdapEntry ldapEntry)
        {

            var attributeVal = ldapEntry.getAttribute("pwdLastSet")?.StringValue ?? "";
            if (attributeVal == "0")
            {
                return "User must change at next logon.";
            }

            return "";
        }

    }
}
