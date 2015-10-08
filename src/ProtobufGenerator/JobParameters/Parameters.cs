using ProtobufGenerator.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ProtobufGenerator.JobParameters
{
    /// <summary>
    /// Represents a collection of processing Jobs. 
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// A collection of Jobs to process
        /// </summary>
        public IEnumerable<Job> Jobs { get; set; }

        /// <summary>
        /// The solution directory is the reference point for include file paths in the Job parameters
        /// </summary>
        public string SolutionDirectory { get; set; }

        /// <summary>
        /// Parse a valid Parameters object from an XDocument, validating with a provided schema
        /// </summary>
        /// <param name="document">The XDocument instance representing the Parameters object</param>
        /// <param name="schema">The XML Schema to validate the XDocument</param>
        /// <returns></returns>
        internal static Parameters FromXDocument(XDocument document, XmlSchema schema)
        {
#if DNX451  // DNXCore50 doesn't yet have capability to validate XML Schema, cross fingers and pray. 
            // Validate the XDocument
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);
            document.Validate(schemaSet, null);
#endif
            // Preparse elements needed to process other elements and those difficult to process inline.
            var solutionDirectory = document.Descendants("SolutionDirectory").Single().Value;

            var jobElements = document.Descendants("Job");

            var customAnnotations = new Dictionary<string, IList<string>>();
            foreach (var annotation in jobElements.Select(job => job.SafeElement("CustomAnnotations")
                .Elements("CustomAnnotation")).SelectMany(ann =>
                {
                    var xElements = ann as IList<XElement> ?? ann.ToList();
                    return xElements;
                }))
            {
                if (customAnnotations.ContainsKey(annotation.Attribute("codeElement").Value))
                {
                    customAnnotations[annotation.Attribute("codeElement").Value].Add(annotation.Value);
                }
                else
                {
                    customAnnotations.Add(annotation.Attribute("codeElement").Value, new List<string> { annotation.Value });
                }
            }

            // Create and return a new Parameters object
            return new Parameters
            {
                SolutionDirectory = solutionDirectory,
                Jobs = document.Descendants("Job").Select(x => new Job
                {
                    Name = (string)x.Attribute("name"),
                    DestinationNamespace = (string)x.Element("Namespace"),
                    ProtoDirectory = Path.Combine(solutionDirectory, (string)x.Element("ProtoDirectory")),
                    ImportDirectories = x.SafeElement("Imports").Elements("Directory").Select(e => Path.Combine(solutionDirectory, e.Value)),
                    NullableClasses = x.SafeElement("UseNullable").Elements("NullableClass").Select(e => e.Value),
                    CustomAnnotations = customAnnotations,
                    OutputDirectory = Path.Combine(solutionDirectory, (string)x.Element("OutputDirectory")),
                    DestinationProject = (string)x.Element("Project")
                })
            };
        }
    }
}
