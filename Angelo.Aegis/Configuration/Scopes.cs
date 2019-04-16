using IdentityModel;
using System.Collections.Generic;

using IdentityServer4.Models;

namespace Angelo.Aegis.Configuration
{
    public static class Scopes
    {

        public static Scope OpenId = new Scope()
        {
            Name = "openid",
            DisplayName = "Your user identity",
            Description = "Identity information about a user.",
            UserClaims =
            {
                "sub",
                "user_id",
                "user_name",
                "name",
                "idp",
                "auth_time",
                "lockout_end",
                "access_token",
                "email",
                "email_verified"
            }
        };

        public static Scope Profile = new Scope()
        {
            Name = "profile",
            DisplayName = "User profile",
            Description = "Your user profile information.",
            UserClaims = new IdentityResources.Profile().UserClaims
        };

        public static Scope Email = new Scope()
        {
            Name = "email",
            DisplayName = "User Email",
            Description = "Your email address.",
            UserClaims = {
                "email",
                "email_verified"
            }
        };

        public static Scope Security = new Scope()
        {
            Name = "security",
            DisplayName = "Security Claims",
            Description = "Roles, permissions, and memberships",
            UserClaims =
            {
                "pool",
                "role",
                "tenant",
                "permission"
            }
        };

        public static Scope Drive = new Scope()
        {
            Name = "drive",
            DisplayName = "Angelo Drive",
            Description = "Cloud Based Document Storage",
            UserClaims =
            {
                "file",
                "folder"
            }
        };

        public static IdentityResource ToIdentityResource(this Scope scope)
        {
            return new IdentityResource
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                UserClaims = scope.UserClaims,
            };
        }

        public static ApiResource ToApiResource(this Scope scope, Secret secret, ICollection<Scope> scopes)
        {
            return new ApiResource
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                UserClaims = scope.UserClaims,
                Scopes = scopes,
                ApiSecrets =
                {
                    secret
                },
            };
        }
    }

}