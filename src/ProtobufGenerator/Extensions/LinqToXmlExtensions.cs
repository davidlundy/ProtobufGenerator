using System.Xml.Linq;

namespace ProtobufGenerator.Extensions
{
    public static class LinqToXmlExtensions
    {
        /// <summary>
        /// Provide null safe access to an XElement, returning an empty XElement instead of 
        /// null. 
        /// </summary>
        /// <param name="element">The parent XElement instance which is the target of the extension method</param>
        /// <param name="name">The name of the child XElement to try to retrieve</param>
        /// <returns></returns>
        public static XElement SafeElement(this XElement element, XName name)
        {
            return element.Element(name) ?? new XElement(name);
        }
    }
}
