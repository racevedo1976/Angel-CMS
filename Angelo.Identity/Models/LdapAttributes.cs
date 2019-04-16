using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Angelo.Identity.Models
{
    public enum LdapAttributes
    {
        [Description("displayName")]
        DisplayName,
        [Description("givenName")]
        FirstName,
        [Description("sn")] // Surname
        LastName,
        [Description("description")]
        Description,
        [Description("telephoneNumber")]
        TelephoneNumber,
        [Description("uid")] // Also used as user name
        Name,
        [Description("uid")]
        UserName,
        [Description("mail")]
        EMail,
        [Description("userPrincipalName")]
        EMailFromPrincipal,
        [Description("memberOf")] // Groups attribute that can appears multiple time
        MemberOf,
        [Description("objectGUID")] 
        LdapGuid,
        [Description("sAMAccountName")]
        sAMAccountName

    }

    public static class LdapAttributesExtensions
    {
        /// <summary>
        /// Create from an <see cref="Enum"/> the description array.
        /// </summary>
        /// <typeparam name="T">An enum type</typeparam>
        /// <returns>An Array of the descriptions (no duplicate)</returns>
        /// <exception cref="ArgumentException">T must be an enumerated type</exception>
        public static Array ToDescriptionArray<T>()
            where T : IConvertible //,struct
        {
            //if (!typeof(T).IsEnum)
            //{
            //    throw new ArgumentException("T must be an enumerated type");
            //}

            List<string> result = new List<string>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var fi = e.GetType().GetField(e.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var description = attributes[0].Description;
                if (!result.Contains(description))
                {
                    result.Add(description);
                }
            }

            return result.ToArray();
        }

        public static string ToDescriptionString(this LdapAttributes val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
