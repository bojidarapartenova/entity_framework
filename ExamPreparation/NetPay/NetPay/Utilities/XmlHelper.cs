using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetPay.Utilities
{
    public class XmlHelper
    {
        public static T? Deserialize<T>(string xml, string rootName)
            where T : class
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            StringReader reader = new StringReader(xml);

            object? deserializedObject = xmlSerializer
                .Deserialize(reader);

            if (deserializedObject == null)
            {
                return null;
            }

            return (T)deserializedObject;
        }

        public static T? Deserialize<T>(Stream xml, string rootName)
            where T : class
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            object? deserializedObject = xmlSerializer
                .Deserialize(xml);

            if (deserializedObject == null)
            {
                return null;
            }

            return (T)deserializedObject;
        }

        public static string Serialize<T>(T obj, string rootName, Dictionary<string, string>? namespaces = null)
        {
            StringBuilder result = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmLSerializer = new XmlSerializer(typeof(T), xmlRoot);

            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            if (namespaces == null)
            {
                xmlNamespaces.Add(string.Empty, string.Empty);
            }
            else
            {
                foreach (var ns in namespaces)
                {
                    xmlNamespaces.Add(ns.Key, ns.Value);
                }
            }

            using StringWriter stringWriter = new StringWriter(result);

            xmLSerializer.Serialize(stringWriter, obj, xmlNamespaces);

            return result.ToString().TrimEnd();
        }

        public static void Serialize<T>(T obj, string rootName, Stream output, Dictionary<string, string>? namespaces = null)
        {
            StringBuilder result = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmLSerializer = new XmlSerializer(typeof(T), xmlRoot);

            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            if (namespaces == null)
            {
                xmlNamespaces.Add(string.Empty, string.Empty);
            }
            else
            {
                foreach (var ns in namespaces)
                {
                    xmlNamespaces.Add(ns.Key, ns.Value);
                }
            }

            xmLSerializer.Serialize(output, obj, xmlNamespaces);
        }
    }
}
