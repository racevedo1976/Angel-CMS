using System;
using System.Security.Cryptography;

namespace Angelo
{
    public static class KeyGen
    {
        // Base 32 characters used for shortened guids
        // Some confusing chars are ignored: http://www.crockford.com/wrmg/base32.html
        private static readonly string _base32chars = "0123456789abcdefghjkmnpqrstvwxyz";


        /// <summary>
        /// Returns a system guid as a normalized base 16 string
        /// </summary>
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString("N").ToLower();
        }

        /// <summary>
        /// Returns a system guid as a normalized base 32 string
        /// </summary>
        public static string NewGuid32()
        {
            // Generate a base32 Guid value
            var guid = System.Guid.NewGuid().ToByteArray();

            var hs = BitConverter.ToInt64(guid, 0);
            var ls = BitConverter.ToInt64(guid, 8);

            return ToBase32(hs, ls);
        }


        private static string ToBase32(long hs, long ls)
        {
            var charBuffer = new char[26];

            charBuffer[0] = _base32chars[(int)(hs >> 60) & 31];
            charBuffer[1] = _base32chars[(int)(hs >> 55) & 31];
            charBuffer[2] = _base32chars[(int)(hs >> 50) & 31];
            charBuffer[3] = _base32chars[(int)(hs >> 45) & 31];
            charBuffer[4] = _base32chars[(int)(hs >> 40) & 31];
            charBuffer[5] = _base32chars[(int)(hs >> 35) & 31];
            charBuffer[6] = _base32chars[(int)(hs >> 30) & 31];
            charBuffer[7] = _base32chars[(int)(hs >> 25) & 31];
            charBuffer[8] = _base32chars[(int)(hs >> 20) & 31];
            charBuffer[9] = _base32chars[(int)(hs >> 15) & 31];
            charBuffer[10] = _base32chars[(int)(hs >> 10) & 31];
            charBuffer[11] = _base32chars[(int)(hs >> 5) & 31];
            charBuffer[12] = _base32chars[(int)hs & 31];

            charBuffer[13] = _base32chars[(int)(ls >> 60) & 31];
            charBuffer[14] = _base32chars[(int)(ls >> 55) & 31];
            charBuffer[15] = _base32chars[(int)(ls >> 50) & 31];
            charBuffer[16] = _base32chars[(int)(ls >> 45) & 31];
            charBuffer[17] = _base32chars[(int)(ls >> 40) & 31];
            charBuffer[18] = _base32chars[(int)(ls >> 35) & 31];
            charBuffer[19] = _base32chars[(int)(ls >> 30) & 31];
            charBuffer[20] = _base32chars[(int)(ls >> 25) & 31];
            charBuffer[21] = _base32chars[(int)(ls >> 20) & 31];
            charBuffer[22] = _base32chars[(int)(ls >> 15) & 31];
            charBuffer[23] = _base32chars[(int)(ls >> 10) & 31];
            charBuffer[24] = _base32chars[(int)(ls >> 5) & 31];
            charBuffer[25] = _base32chars[(int)ls & 31];

            return new string(charBuffer);
        }
    }
}