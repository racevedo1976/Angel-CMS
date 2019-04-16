using System;
using System.Collections.Generic;


namespace Angelo.Connect.Extensions
{
    public class EncodedParamList : Dictionary<string, string>
    {
        public string AsEncodedStr
        {
            get { return GetEncodedString();  }
            set { SetEncodedString(value); }
        }

        protected string GetEncodedString()
        {
            var list = new List<string>();
            foreach (var item in this)
                list.Add(item.Key + "=" + item.Value);
            var text = string.Join("\t", list.ToArray());
            return "00" + text.AsciiToHex().StrReverse();
        }

        protected void SetEncodedString(string encodedStr)
        {
            this.Clear();
            if (!string.IsNullOrEmpty(encodedStr))
            {
                if (encodedStr.Substring(0, 2) == "00")
                {
                    string dataStr;
                    try
                    {
                        dataStr = encodedStr.Substring(2).StrReverse().HexToAscii();
                    }
                    catch
                    {
                        dataStr = "";
                    }
                    var list = dataStr.Split('\t');
                    foreach (var pair in list)
                    {
                        var pos1 = pair.IndexOf('=');
                        if (pos1 > -1)
                        {
                            var key = pair.Substring(0, pos1).Trim();
                            var value = pair.Substring(pos1 + 1);
                            this.Add(key, value);
                        }
                    }
                }
            }
        }

        public string GetValueOrDefault(string name, string defaultValue)
        {
            String value;
            if (this.TryGetValue(name, out value))
                return value;
            else
                return defaultValue;
        }



    }
}