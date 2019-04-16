using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Services
{
    public class SignInResultCustom
    {
       
        private SignInResultCustom()
        {

        }

        public SignInResultCustom(Models.User user, Microsoft.AspNetCore.Identity.SignInResult result)
        {
            User = user;
            Succeeded = result.Succeeded;
            IsLockedOut = result.IsLockedOut;
            IsNotAllowed = result.IsNotAllowed;
            RequiresTwoFactor = result.RequiresTwoFactor;
        }

        public Models.User User { get; protected set; }

        /// <summary>
        /// Returns a flag indication whether the sign-in was successful.
        /// </summary>
        /// <value>True if the sign-in was successful, otherwise false.</value>
        public bool Succeeded { get; protected set; }

        /// <summary>
        /// Returns a flag indication whether the user attempting to sign-in is locked out.
        /// </summary>
        /// <value>True if the user attempting to sign-in is locked out, otherwise false.</value>
        public bool IsLockedOut { get; protected set; }

        /// <summary>
        /// Returns a flag indication whether the user attempting to sign-in is not allowed to sign-in.
        /// </summary>
        /// <value>True if the user attempting to sign-in is not allowed to sign-in, otherwise false.</value>
        public bool IsNotAllowed { get; protected set; }

        /// <summary>
        /// Returns a flag indication whether the user attempting to sign-in requires two factor authentication.
        /// </summary>
        /// <value>True if the user attempting to sign-in requires two factor authentication, otherwise false.</value>
        public bool RequiresTwoFactor { get; protected set; }

        /// <summary>
        /// Returns a flag indication whether the user attempting to sign-in is required to change their password.
        /// </summary>
        /// <value>True if the user attempting to sign-in must change their password, otherwise false.</value>
        public bool MustChangePassword { get; protected set; }

        /// <summary>
        /// Returns a flag indication whether the user attempting to sign-in has a confirmed email address.
        /// </summary>
        /// <value>True if the user attempting to sign-in has a confirmed email address, otherwise false.</value>
        public bool IsEmailNotConfirmed { get; protected set; }

        /// <summary>
        /// Returns a <see cref="SignInResultCustom"/> that represents a successful sign-in.
        /// </summary>
        /// <returns>A <see cref="SignInResultCustom"/> that represents a successful sign-in.</returns>
        public static SignInResultCustom Success(Models.User user)
        {
            return new SignInResultCustom {  User = user, Succeeded = true };
        }

        /// <summary>
        /// Returns a <see cref="SignInResultCustom"/> that represents a failed sign-in.
        /// </summary>
        /// <returns>A <see cref="SignInResultCustom"/> that represents a failed sign-in.</returns>
        public static SignInResultCustom Failed(Models.User user = null)
        {
            return new SignInResultCustom { User = user };
        }

        /// <summary>
        /// Returns a <see cref="SignInResultCustom"/> that represents a sign-in attempt that failed because 
        /// the user was logged out.
        /// </summary>
        /// <returns>A <see cref="SignInResultCustom"/> that represents sign-in attempt that failed due to the
        /// user being locked out.</returns>
        public static SignInResultCustom LockedOut(Models.User user = null)
        {
            return new SignInResultCustom {  User = user, IsLockedOut = true };
        }

        /// <summary>
        /// Returns a <see cref="SignInResultCustom"/> that represents a sign-in attempt that failed because 
        /// the user is not allowed to sign-in.
        /// </summary>
        /// <returns>A <see cref="SignInResultCustom"/> that represents sign-in attempt that failed due to the
        /// user is not allowed to sign-in.</returns>
        public static SignInResultCustom NotAllowed(Models.User user = null)
        {
            return new SignInResultCustom { User = user, IsNotAllowed = true };
        }

        /// <summary>
        /// Returns a <see cref="SignInResultCustom"/> that represents a sign-in attempt that needs two-factor 
        /// authentication.
        /// </summary>
        /// <returns>A <see cref="SignInResultCustom"/> that represents sign-in attempt that needs two-factor
        /// authentication.</returns>
        public static SignInResultCustom TwoFactorRequired(Models.User user = null)
        {
            return new SignInResultCustom { User = user, RequiresTwoFactor = true };
        }


        /// <summary>
        /// Returns a <see cref="SignInResultCustom"/> that represents a sign-in attempt that must change password
        /// authentication.
        /// </summary>
        public static SignInResultCustom ChangePasswordRequired(Models.User user = null)
        {
            return new SignInResultCustom { User = user, MustChangePassword = true };
        }

        /// <summary>
        /// Returns a <see cref="SignInResultCustom"/> that represents a sign-in attempt that failed because the email
        /// has not been confirmed.
        /// </summary>
        public static SignInResultCustom EmailNotConfirmed(Models.User user = null)
        {
            return new SignInResultCustom { User = user, IsEmailNotConfirmed = true };
        }

        /// <summary>
        /// Converts the value of the current <see cref="SignInResultCustom"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of value of the current <see cref="SignInResultCustom"/> object.</returns>
        public override string ToString()
                {
                    return IsLockedOut ? "Lockedout" :
                           IsNotAllowed ? "NotAllowed" :
                           RequiresTwoFactor ? "RequiresTwoFactor" :
                           MustChangePassword ? "MustChangePassword" :
                           IsEmailNotConfirmed ? "Email has not been confirmed." :
                           Succeeded ? "Succeeded" : "Invalid login attempt.";
                }
    }
}
