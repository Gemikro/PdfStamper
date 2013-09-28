using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Pdf.Common
{
    public static class XmlUtils
    {

        public static string SerializeXml(this object o) {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            try {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(o.GetType());
                ser.Serialize(sw, o);
            }
            finally {
                sw.Flush();
                sw.Close();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Deserialization.
        /// </summary>
        /// <param name="s">String containing XML serialization</param>
        /// <param name="t">Type to be deserialized</param>
        /// <returns>Deserialized object - has to be typecasted later.</returns>

        private static object DeserializeXml(string s, Type t) {
            StringReader sr = new StringReader(s);
            try {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(t);
                return ser.Deserialize(sr);
            }
            finally { sr.Close(); }
        }

        /// <summary> Wrapper for DeserializeXml </summary>
        /// <param name="xml_doc">Xml document</param>
        /// <param name="t">Type to be deserialized</param>
        /// <returns>Deserialized object - has to be typecasted later.</returns>

        private static object DeserializeXml(XmlDocument xml_doc, Type t) {
            return DeserializeXml(xml_doc.OuterXml, t);
        }

        /// <summary> Wrapper for DeserializeXml </summary>
        /// <param name="xml_doc">Xml document</param>
        /// <typeparam name="T">Type to be deserialized</typeparam>
        /// <returns>Deserialized object - has to be typecasted later.</returns>

        public static T DeserializeXml2<T>(this XmlDocument xml_doc) {
            return (T)DeserializeXml(xml_doc.OuterXml, typeof(T));
        }

        /// <summary> Wrapper for DeserializeXml </summary>
        /// <param name="xml_string">Xml serialized in string</param>
        /// <typeparam name="T">Type to be deserialized</typeparam>
        /// <returns>Deserialized object.</returns>

        public static T DeserializeXml2<T>(this string xml_string) {
            return (T)DeserializeXml(xml_string, typeof(T));
        }
    }
}
