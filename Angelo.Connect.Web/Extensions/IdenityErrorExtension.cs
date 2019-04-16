using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.Extensions
{
    public static class IdenityErrorExtension
    {
        public static string ToString(IEnumerable<IdentityError> errors)
        {
            return String.Join(". ", errors.Select(x => x.Description).ToArray());
        }
    }
}
