using System;
using System.Collections.Generic;


using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace Utilities
{

    public class XmlObjectSerializer
    {
        public static readonly XmlObjectSerializer Default = new XmlObjectSerializer();

        XmlSerializerNamespaces Namespaces;

        Dictionary<Type, XmlSerializer> Serializers;
    
        public XmlObjectSerializer()
        {
            Namespaces = new XmlSerializerNamespaces();
            Namespaces.Add("", "");

            Serializers = new Dictionary<Type, XmlSerializer>();
        }

        object SerializersLock = new object();

        XmlSerializer CreateSerializer<T>()
        {
            Type t = typeof(T);

            XmlSerializer serializer;
            lock (SerializersLock)
            {
                if (!Serializers.TryGetValue(t, out serializer))
                {
                    serializer = new XmlSerializer(t);
                    Serializers.Add(t, serializer);
                }
            }
            return serializer;
        }

        public string Serialize<T>(T data)
        {
            var serializer = CreateSerializer<T>();
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            serializer.Serialize(writer, data, Namespaces);
            return sb.ToString();
        }

        public void Serialize<T>(T data, Stream stream)
        {
            var serializer = CreateSerializer<T>();
            serializer.Serialize(stream, data, Namespaces);
        }

        public T Deserialize<T>(Stream xml)
        {
            var serializer = CreateSerializer<T>();
            T data = (T)serializer.Deserialize(xml);
            return data;
        }
        public T Deserialize<T>(string xml)
        {
            var serializer = CreateSerializer<T>();
            T data = (T)serializer.Deserialize(new StringReader(xml));
            return data;
        }
    }
}