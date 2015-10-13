using System.Collections.Generic;

namespace ProtobufGenerator.Configuration
{
    /// <summary>
    /// Represents a collection of .proto processing <see cref="Job"/>s
    /// </summary>
    public class JobSet
    {
        /// <summary>
        /// A collection of Jobs to process
        /// </summary>
        public IEnumerable<Job> Jobs { get; set; }

        /// <summary>
        /// The solution directory is the reference point for include file paths in the Job parameters
        /// </summary>
        public string SolutionDirectory { get; set; }
    }
}
