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
        //[DeploymentItem("TestFiles\\TestParameters.xml")]
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
        /// expected exception.
        /// </summary>
        [Fact]
        //[DeploymentItem("TestFiles\\WontValidate.xml")]
        //[ExpectedException(typeof(XmlSchemaValidationException))]
        public void ThrowsWhenFailsValidation()
        {
            var engine = new ProtoEngine();
            const string testXml = @"TestFiles\WontValidate.xml";
            Assert.True(File.Exists(testXml));
            Assert.Throws<XmlSchemaValidationException>(() => engine.LoadParameters(testXml));
            //Assert.NotNull(engine.Parameters);
        }
    }
}
