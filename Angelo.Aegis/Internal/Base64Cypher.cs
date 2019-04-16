using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Internal
{
    //TODO: Replace with real 2-way cypher
    //NOTE: We'll possibly need to pass these values around in urls so needs to be url safe

    public class Base64Cypher : ICypher
    {
        public string Cypher(string value)
        {
            var bytes = ASCIIEncoding.ASCII.GetBytes(value);

            return Convert.ToBase64String(bytes);
        }

        public string Cypher(string value, string salt)
        {
            // ignoring salt for now - this is to be rewritten anyway
            return Cypher(value); 
        }

        public string Decypher(string value)
        {
            string result = null;
            try
            {
                var bytes = Convert.FromBase64String(value);

                result = ASCIIEncoding.ASCII.GetString(bytes);
            }
            finally{ }

            return result; 
        }

        public string Decypher(string value, string salt)
        {
            // ignoring salt for now - this is to be rewritten anyway
            return Decypher(value); 
        }

    }
}
