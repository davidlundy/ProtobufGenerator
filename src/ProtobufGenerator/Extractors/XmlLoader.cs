using ProtobufGenerator.Interfaces;
using System.Xml.Linq;
using System;

namespace ProtobufGenerator.Extractors
{
    /// <summary>
    /// Wrapper class facilitating testing. IXmlLoader will be Mocked. 
    /// </summary>
    public class XmlLoader : IXmlLoader
    {
        public XDocument Load(string xmlpath)
        {
            if(xmlpath == null)
            {
                throw new ArgumentNullException("xmlpath");
            }
            return XDocument.Load(xmlpath);
        }
    }
}
