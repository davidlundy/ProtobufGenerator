using ProtobufGenerator.Configuration;

namespace ProtobufGenerator.Interfaces
{
    /// <summary>
    /// Interface representing a configuration file. Once the configuration support
    /// in Microsoft.Extensions.Configuration stabalizes we should migrate this to that.
    /// Currently namespaces are changing and key functionality is in development and
    /// unpublished so we wait a bit.
    /// ref : https://github.com/aspnet/Configuration
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Defines a list of .proto Jobs to process.
        /// </summary>
        JobSet JobSet { get; }
    }
}