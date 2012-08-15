using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ReSTCore.Util
{
    /// <summary>
    /// A Dictionary that can be serialized by XmlSerializer.
    /// See http://weblogs.asp.net/pwelter34/archive/2006/05/03/444961.aspx
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [XmlRoot("Result")]
    public class PropertyResult<TValue>
        : Dictionary<string, TValue>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            var valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (string key in Keys)
            {
                writer.WriteStartElement(key);
                TValue value = this[key];
                if (value == null)
                {
                    /*Do nothing}*/
                }
                else if (value.GetType().IsPrimitive || value is string)
                    writer.WriteString(value.ToString());
                else
                    valueSerializer.Serialize(writer, value, ns);
                
                writer.WriteEndElement();
            }
        }
    }
}