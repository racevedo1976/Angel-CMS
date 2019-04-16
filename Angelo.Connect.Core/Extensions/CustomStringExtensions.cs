using System;
using System.Text;

namespace Angelo.Connect.Extensions
{
    public static class CustomStringExtension
    {
        public static string StrReverse(this string text)
        {
            char[] charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string AsciiToHex(this string asciiStr)
        {
            var sb = new StringBuilder();
            var bytes = Encoding.ASCII.GetBytes(asciiStr);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString(); // returns: "48656C6C6F20576F726C64" for "Hello world"
        }

        public static string HexToAscii(this string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.ASCII.GetString(bytes); // returns: "Hello world" for "48656C6C6F20576F726C64"
        }

        public static string HexToAsciiDefault(this string hexString, string defaultString)
        {
            try
            {
                return HexToAscii(hexString);
            }
            catch
            {
                return defaultString;
            }
        }

        public static string UnicodeToHex(this string unicodeStr)
        {
            var sb = new StringBuilder();
            var bytes = Encoding.Unicode.GetBytes(unicodeStr);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString(); // returns: "480065006C006C006F00200057006F0072006C006400" for "Hello world"
        }

        public static string HexToUnicode(this string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "480065006C006C006F00200057006F0072006C006400"
        }

        public static string HexToUnicodeDefault(this string hexString, string defaultString)
        {
            try
            {
                return HexToUnicode(hexString);
            }
            catch
            {
                return defaultString;
            }
        }


        public static string Truncate(string source, int length)
        {
            if (!String.IsNullOrEmpty(source))
            {
                if (source.Length > length)
                {
                    source = source.Substring(0, length) + "...";
                }
            }
            return source;
        }

    }


}