using System;

namespace ProtobufCompiler.Polyfill
{
    /// <summary>
    /// Apparently this is now an internal class in the System.Diagnostics namespace.
    /// <para>https://github.com/dotnet/corefx/issues/10685</para>
    /// <para>Seems like a fix is coming for netstandard2.0 but not yet released, so in the meantime we use a polyfill =)</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    [ExcludeFromCodeCoverage("Inception")]
    public sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
        private string _justification;
        public ExcludeFromCodeCoverageAttribute(string justification)
        {
            _justification = justification;
        }
    }
}
