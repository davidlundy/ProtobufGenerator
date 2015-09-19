using System.Reflection;
using System.Xml.Schema;

namespace ProtobufGenerator.Extractors
{
    public class ResourceExtractor
    {
        /// <summary>
        /// Extracts an XMLSchema given the resource name, with the default Protogent.Core.Embedded namespace
        /// </summary>
        /// <param name="resourceName">The name of the resource to extract</param>
        /// <returns></returns>
        public static XmlSchema ExtractSchema(string resourceName)
        {
            return ExtractSchema("ProtobufGenerator.Embedded", resourceName);
        }

        /// <summary>
        /// Extracts an XMLSchema given a resource name and a namespace
        /// </summary>
        /// <param name="resourceLocation">The namespace of the resource</param>
        /// <param name="resourceName">The name of the resource</param>
        /// <returns></returns>
        public static XmlSchema ExtractSchema(string resourceLocation, string resourceName)
        {
            var myAssembly = Assembly.GetExecutingAssembly();
            using (var schemaStream = myAssembly.GetManifestResourceStream(resourceLocation + @"." + resourceName))
            {
                return schemaStream == null ? null : XmlSchema.Read(schemaStream, null);
            }
        }
    }
}
