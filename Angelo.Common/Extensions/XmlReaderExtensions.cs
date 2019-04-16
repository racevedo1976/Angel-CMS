using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Angelo.Common.Extensions
{
    public static class XmlReaderExtensions
    {
        /// <summary>
        /// Reads the next beginning tag element from the XmlReader with a name equal to beginTagName.
        /// An exception is thown if the specified beginning element is not found and read.
        /// </summary>
        public static async Task ReadBeginTagAsync(this XmlReader reader, string beginTagName)
        {

            bool found = false;
            while (found == false && (await reader.ReadAsync()))
                {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(beginTagName, StringComparison.OrdinalIgnoreCase))
                    found = true;
            }
            if (found == false)
                throw new XmlException(string.Format("Beginning tag element </{0}> not found.", beginTagName));
        }

        /// <summary>
        /// Reads the next tag element from the XmlReader.
        /// Returns true if an element is read and is not a closing tag of endTagName.
        /// Returns false if an element is read and is the closing tag of endTagName.
        /// An exception is thown if there are no more times to be read. 
        /// note: You would call this method inside a while condition.
        /// </summary>
        public static async Task<bool> ReadNextTagAsync(this XmlReader reader, string endTagName)
        {
            bool result = true;
            if (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(endTagName, StringComparison.OrdinalIgnoreCase))
                    result = false;
            }
            else
                throw new XmlException(string.Format("Closing </{0}> tag not found.", endTagName));
            return result;
        }

        /// <summary>
        /// Returns true if the current tag element is a begining or opening tag and has the specified name.
        /// </summary>
        public static bool IsBeginTag(this XmlReader reader, string name)
        {
            return (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the attribute of the current element that has the specified attributeName.
        /// If the attribute does not exist, then an exception is thrown.
        /// </summary>
        public static string GetRequiredAttribute(this XmlReader reader, string attributeName)
        {
            string value = reader.GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
                throw new XmlException(string.Format("Attribute \"{0}\" is not defined in tag <{1}>.", attributeName, reader.Name));
            return value;
        }

        /// <summary>
        /// Returns the attribute of the current element that has the specified attributeName.
        /// If the attribute does not exist, then the default value is returned.
        /// </summary>
        public static string GetOptionalAttribute(this XmlReader reader, string attributeName, string defaultValue)
        {
            string value = reader.GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            else
                return value;
        }
    }


}
