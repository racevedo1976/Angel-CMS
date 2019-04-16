using System;

namespace Angelo.Drive
{
    public static class KeyGenerator
    {
        public static string CreateShortGuid()
        {
            var guid = Guid.NewGuid();
            return CreateShortGuid(guid);
        }

        public static string CreateShortGuid(Guid guid)
        {
            var id = Convert.ToBase64String(guid.ToByteArray());
            return id.Replace("=", "").Replace("/","-"); 
        }

        public static Guid FromShortGuid(string shortGuid)
        {
            shortGuid += new String('=', shortGuid.Length % 4);
            shortGuid.Replace("-", "/");
            return new Guid(Convert.FromBase64String(shortGuid));
        }
    }
}
