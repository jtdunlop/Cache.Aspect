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
            return textWriter.ToString();
        }
    }
}