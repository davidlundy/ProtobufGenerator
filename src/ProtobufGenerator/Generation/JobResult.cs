using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
