using Xunit;
using System.IO;
using System.Xml.Schema;

namespace ProtobufGenerator.Tests
{
    public class ParametersTest
    {

        /// <summary>
        /// Test that our loader can validate and load a good Parameters XML file. 
        /// This is the happy case. We'll test unhappiness later. 
        /// </summary>
        [Fact]
        public void CanLoadParametersFromRealFile()
        {
            var engine = new ProtoEngine();
            const string testXml = @"TestFiles\TestParameters.xml";
            Assert.True(File.Exists(testXml));
            engine.LoadParameters(testXml);

            Assert.NotNull(engine.Parameters);
        }

        /// <summary>
        /// Verifies that the loaded XML file will fail validation and throw the 
        /// expected exception. Only valid for .NET Framework full platform until
        /// DNXCore50 supports XmlSchema validation.
        /// </summary>
        [Fact]
        public void ThrowsWhenFailsValidation()
        {
#if DNX451
            var engine = new ProtoEngine();
            const string testXml = @"TestFiles\WontValidate.xml";
            Assert.True(File.Exists(testXml));
            Assert.Throws<XmlSchemaValidationException>(() => engine.LoadParameters(testXml));
#endif
        }
    }
}
