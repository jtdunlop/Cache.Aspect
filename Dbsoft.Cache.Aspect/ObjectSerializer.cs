// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect
{
    using System.IO;
    using System.Xml.Serialization;

    public static class ObjectSerializer
    {
        public static string SerializeObject<T>(this T toSerialize)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType());
            var textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, toSerialize);
            var result = textWriter.ToString();
            return result;
        }

        public static object DeserializeObject(this string toDeserialize)
        {
            var xmlSerializer = new XmlSerializer(toDeserialize.GetType());
            var textReader = new StringReader(toDeserialize);
            var result = xmlSerializer.Deserialize(textReader);
            return result;
        }
    }
}