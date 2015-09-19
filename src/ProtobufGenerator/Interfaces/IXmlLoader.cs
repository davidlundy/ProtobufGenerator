using System.Xml.Linq;

namespace ProtobufGenerator.Interfaces
{
    public interface IXmlLoader
    {
        XDocument Load(string path);
    }
}
