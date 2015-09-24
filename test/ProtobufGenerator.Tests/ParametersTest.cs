using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Schema;

namespace ProtobufGenerator.Tests
{
    [TestClass]
    public class ParametersTest
    {

        /// <summary>
        /// Test that our loader can validate and load a good Parameters XML file. 
        /// This is the happy case. We'll test unhappiness later. 
        /// </summary>
        [TestMethod]
        [DeploymentItem("TestFiles\\TestParameters.xml")]
        public void CanLoadParametersFromRealFile()
        {
            var engine = new ProtoEngine();
            var testXml = "TestParameters.xml";
            Assert.IsTrue(File.Exists(testXml));
            engine.LoadParameters(testXml);

            Assert.IsNotNull(engine.Parameters);
        }

        /// <summary>
        /// Verifies that the loaded XML file will fail validation and throw the 
        /// expected exception.
        /// </summary>
        [TestMethod]
        [DeploymentItem("TestFiles\\WontValidate.xml")]
        [ExpectedException(typeof(XmlSchemaValidationException))]
        public void ThrowsWhenFailsValidation()
        {
            var engine = new ProtoEngine();
            var testXml = "WontValidate.xml";
            Assert.IsTrue(File.Exists(testXml));
            engine.LoadParameters(testXml);

            Assert.IsNotNull(engine.Parameters);
        }
    }
}
