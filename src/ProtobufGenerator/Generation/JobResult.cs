using System.IO;

namespace ProtobufGenerator.Generation
{
    public class JobResult
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string Directory { get; set; }
        public string FilePath => Path.Combine(Directory, FileName);
    }
}
