using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity
{
    //TODO: Inherit from JWTClaimsTypes in IdentityModels namespace
    public static class ClaimType
    {
        // based mostly on JWT Claim Types with some additional
        public static string Id { get; } = "user_id";
        public static string Subject { get; } = "sub";
        public static string UserName { get; } = "user_name";
        public static string Name { get; } = "name";
        public static string Role { get; } = "role";
        public static string Group { get; set; } = "group";
        public static string Email { get; } = "email";
        public static string EmailConfirmed { get; } = "email_verified";
        public static string DisplayName { get; } = "display_name";     
        public static string FirstName { get; } = "given_name";
        public static string LastName { get; } = "family_name";
        public static string Gender { get; } = "gender";
        public static string Birthday { get; } = "birthday";
        public static string Title { get; } = "title";
        public static string Suffix { get; } = "suffix";
        public static string PhoneNumber { get; } = "phone_number";
        public static string PhoneNumberConfirmed { get; } = "phone_number_verified";
        public static string AltId { get; } = "alt_id";
        public static string SecurityStamp { get; } = "security_stamp";
        public static string ExternalLoginProvider { get; } = "external_login_provider";
        public static string LockoutEnd { get; } = "lockout_end";
        public static string Locale { get; } = "locale";
        public static string TimeZone { get; } = "zoneinfo";
        public static string IdentityProvider { get; } = "idp";
        public static string MembershipPool { get; } = "pool";
        public static string Tenant { get; } = "tenant";
        public static string AuthenticationTime { get; } = "auth_time";


    }
}
