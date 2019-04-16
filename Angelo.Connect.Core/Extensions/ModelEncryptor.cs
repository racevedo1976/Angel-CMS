using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Extensions
{
    public static class ModelEncryptor
    {
        public static string Encrypt<TModel>(TModel model) where TModel : class
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return "00" + json.AsciiToHex().StrReverse(); // Add the 00 format header
        }

        public static TModel Decrypt<TModel>(string encryptedStr) where TModel : class
        {
            if (encryptedStr.Substring(0, 2) == "00") // read and validate the header
            {
                var json = encryptedStr.Substring(2).StrReverse().HexToAscii();
                TModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<TModel>(json);
                return model;
            }
            else
                throw new Exception("Invalid encryption format.");
        }

    }
}
