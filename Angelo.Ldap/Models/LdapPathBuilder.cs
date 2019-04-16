using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angelo.Ldap
{
    class LdapPathBuilder
    {
        const string _singleForwardSlash = @"/";
        const string _doubleForwardSlash = @"//";
        const string _pathPrefix = @"LDAP:" + _doubleForwardSlash;

        private string _host;
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        private string _DN;
        public string DN
        {
            get { return _DN; }
            set { _DN = value; }
        }

        public string Path
        {
            get { return ComposePath(_host, _DN); }
            set { SetPath(value); }
        }

        public LdapPathBuilder()
        {
        }

        public LdapPathBuilder(string path)
        {
            SetPath(path);
        }

        /// <summary>
        /// Parses and saves the domain (Host) and distinguished name (DN) from the specified path.
        /// ex: LDAP://mydomain.com/dc=mydomain,cd=com
        /// </summary>
        private void SetPath(string path)
        {
            string newHost;
            string newDN;
            string newData;
            if (!path.StartsWith(_pathPrefix, StringComparison.CurrentCultureIgnoreCase))
                throw new FormatException($"LDAP Path missing {_pathPrefix} prefix.");
            int startPos = _pathPrefix.Length;
            int slashPos = path.Replace(@"\/", "..").LastIndexOf(_singleForwardSlash);
            if (startPos < slashPos)
            {
                newHost = path.Substring(startPos, (slashPos - startPos));
                newDN = path.Substring(slashPos + 1);
            }
            else
            {
                newData = path.Substring(startPos);
                if (newData.Contains("="))
                {
                    newHost = string.Empty;
                    newDN = newData;
                }
                else
                {
                    newHost = newData;
                    newDN = string.Empty;
                }
            }
            _host = newHost;
            _DN = newDN;
        }

        /// <summary>
        /// Creates a valid LDAP path with the current host and specified distinguishedName.
        /// </summary>
        /// <param name="distinguishedName">The Distinguished Name of the LDAP root object to be searched or accessed.</param>
        /// <returns></returns>
        public string ComposePath(string distinguishedName)
        {
            return ComposePath(Host, distinguishedName);
        }

        /// <summary>
        /// Call this method to create a valid LDAP path string.
        /// For example: LDAP://mydomain.com/dc=mydomain,cd=com
        /// </summary>
        /// <param name="host">The IP address or domain name of the LDAP server.</param>
        /// <param name="distinguishedName">The Distinguished Name of the LDAP root object to be searched or accessed.</param>
        /// <returns></returns>
        public static string ComposePath(string host, string distinguishedName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_pathPrefix);
            if (!String.IsNullOrEmpty(host))
            {
                sb.Append(host);
            }
            if (!String.IsNullOrEmpty(distinguishedName))
            {
                if (!host.EndsWith(_singleForwardSlash))
                    sb.Append(_singleForwardSlash);
                sb.Append(distinguishedName);
            }
            return sb.ToString();
        }
    }


}
