using System.Threading.Tasks;

namespace ProtobufGenerator.Interfaces
{
    public interface IGenerateProto
    {
        /// <summary>
        /// Process a JobSet that has been initialized in the Engine implementation
        /// </summary>
        void ProcessProto();
    }
}
