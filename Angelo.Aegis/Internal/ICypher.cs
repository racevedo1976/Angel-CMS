using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Internal
{
    public interface ICypher
    {
        string Cypher(string value);
        string Cypher(string value, string salt);
        string Decypher(string value);
        string Decypher(string value, string salt);
    }
}
