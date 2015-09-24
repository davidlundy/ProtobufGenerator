using System.IO;

namespace ProtobufGenerator.JobParameters
{
    /// <summary>
    /// Class representing a Job result. Cached in a dictionary before write out. 
    /// </summary>
    public class JobResult
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string Directory { get; set; }

        public string FilePath
        {
            get { return Path.Combine(Directory, FileName); }
        }
    }
}
