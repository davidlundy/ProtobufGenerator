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
#if DNXCORE50
            var myAssembly = typeof(ResourceExtractor).GetTypeInfo().Assembly;
            return null; // We can't do anything as XmlSchema is an empty type in DNXCore50 meant only to keep an interface from breaking.
#else
            var myAssembly = Assembly.GetExecutingAssembly();

            using (var schemaStream = myAssembly.GetManifestResourceStream(resourceLocation + @"." + resourceName))
            {
                XmlSchema.Read(schemaStream, null);
                return schemaStream == null ? null : XmlSchema.Read(schemaStream, null);
            }
#endif
        }
    }
}
