using ProtobufGenerator.Interfaces;
using System;
using System.Xml.Schema;
using ProtobufGenerator.JobParameters;
using ProtobufGenerator.Extractors;
using System.Collections.Generic;

namespace ProtobufGenerator
{
    public class ProtoEngine : IGenerateProto
    {
        private readonly XmlSchema _xmlSchema;
        private readonly XmlSchemaSet _xmlSchemaSet;
        private readonly IXmlLoader _xmlLoader;

        private readonly Dictionary<string, JobResult> _jobResults = new Dictionary<string, JobResult>();

        public Parameters Parameters { get; private set; }

        public ProtoEngine() 
            : this(new XmlLoader())
        {
            
        }

        /// <summary>
        /// Creates an instance of the <see cref="ProtoEngine"/>, extracts the Parameters.xsd schema 
        /// to the current AppDomain directory, and stores it for later validation. 
        /// </summary>
        public ProtoEngine(IXmlLoader xmlLoader)
        {
            _xmlLoader = xmlLoader;
            _xmlSchema = ResourceExtractor.ExtractSchema("Parameters.xsd");
            _xmlSchemaSet = new XmlSchemaSet();
            _xmlSchemaSet.Add(_xmlSchema);
        }

        /// <summary>
        /// Loads and validates the XML file containing the Parameters description. 
        /// </summary>
        /// <param name="path">Location of the Parameters XML file</param>
        /// <exception cref="XmlSchemaValidationException">Thrown when the XML file fails validation.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the input path is null</exception>
        public void LoadParameters(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("xmlpath");
            }

            var xmlDoc = _xmlLoader.Load(path);
            xmlDoc.Validate(_xmlSchemaSet, null);
            Parameters = Parameters.FromXDocument(xmlDoc, _xmlSchema);
        }

        public void ProcessProto()
        {
            throw new NotImplementedException();
        }

        public void ProcessProto(Parameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
