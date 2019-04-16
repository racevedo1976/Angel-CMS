using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


public static class UserExtensions
{
    public static bool IsSignedIn(this ClaimsPrincipal user)
    {
        return user?.Identity != null ? user.Identity.IsAuthenticated : false;        
    }

    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user?.FindFirst("sub")?.Value;
    }

    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user?.Identity?.Name ?? user?.FindFirst("name")?.Value;
    }

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
    {
        return user?.FindAll("role").Select(x => x.Value);
    }

    public static string GetAccessToken(this ClaimsPrincipal user)
    {
        return user?.FindFirst("access_token")?.Value;
    }


    public static bool IsInAnyRole(this ClaimsPrincipal user, params string[] roles)
    {
        return roles.Any(role => user.IsInRole(role));
    }

    public static bool IsInAllRoles(this ClaimsPrincipal user, params string[] roles)
    {
        return roles.All(role => user.IsInRole(role));
    }
}

