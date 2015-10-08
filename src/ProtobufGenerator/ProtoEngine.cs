using ProtobufGenerator.Interfaces;
using System;
using ProtobufGenerator.JobParameters;
using ProtobufGenerator.Extractors;
using System.Collections.Generic;
using System.Xml.Schema;


namespace ProtobufGenerator
{
    public class ProtoEngine : IGenerateProto
    {
        private readonly XmlSchema _xmlSchema;
#if DNX451
        private readonly XmlSchemaSet _xmlSchemaSet;
#endif
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
#if DNX451
            _xmlSchema = ResourceExtractor.ExtractSchema("Parameters.xsd");
            _xmlSchemaSet = new XmlSchemaSet();
            _xmlSchemaSet.Add(_xmlSchema);
#endif
        }

        /// <summary>
        /// Loads and validates the XML file containing the Parameters description. 
        /// </summary>
        /// <param name="path">Location of the Parameters XML file</param>
        /// <exception cref="XmlSchemaValidationException">Thrown when the XML file fails validation.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the input path is null</exception>
        public void LoadParameters(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            var xmlDoc = _xmlLoader.Load(path);

#if DNX451  // Currently DNXCore50 doesn't offer XML Schema validation.
            xmlDoc.Validate(_xmlSchemaSet, null);
#endif
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
