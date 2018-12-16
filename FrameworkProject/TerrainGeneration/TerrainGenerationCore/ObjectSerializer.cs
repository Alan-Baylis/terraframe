using System;
using System.IO;

namespace TerrainGeneration_Core
{
    class ObjectSerializer
    {
        public static string Serialize(object obj)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());

            StringWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, obj);
            return textWriter.ToString();
        }

        public static Object DeSerialize(string data, Type objectType)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(objectType);

            TextReader reader = new StringReader(data);
            return serializer.Deserialize(reader);
        }
    }
}
