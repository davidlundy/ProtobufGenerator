using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtobufGenerator.Interfaces;
using Moq;
using System.IO;
using System.Xml.Schema;
using System.Xml.Linq;

namespace ProtobufGenerator.Tests
{
    [TestClass]
    public class ParametersTest
    {

        private string _parametersString = "<Parameter><SolutionDirectory>C:\\Projects\\ProtobufGenerator</SolutionDirectory><Job name = \"JobOne\" >< ProtoDirectory > TestDirectory </ ProtoDirectory >< Imports >< Directory > Proto\\EnumImports</Directory><Directory>Proto\\MessageImports</Directory></Imports><UseNullable><NullableClass>MessageMetadata</NullableClass></UseNullable><CustomAnnotations><CustomAnnotation codeElement = \"MessageMetadata\" >[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute] </ CustomAnnotation >< CustomAnnotation codeElement=\"MessageMetadata\">[global::System.Diagnostics.DebuggerDisplay(\"Source={Source}, Destination={Destination}\")]</CustomAnnotation></CustomAnnotations><OutputDirectory>Domain</OutputDirectory><Project>TestProject</Project><Namespace>TestProject</Namespace></Job></Parameter>";

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
