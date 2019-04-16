using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Angelo.Identity.Models;

namespace Angelo.Identity
{
    public class PasswordHasher : PasswordHasher<User> 
    {

        public override string HashPassword(User user, string password)
        {
            return base.HashPassword(user, password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == providedPassword)
            {
                return PasswordVerificationResult.Success;
            }
            else if (!Regex.IsMatch(hashedPassword, @"^[a-zA-Z0-9\+/]*={0,2}$")) 
            {
                return PasswordVerificationResult.Failed; // must be base64 or will throw error below
            }
            else
            {
                return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
            }
        }
    }
}