using System.IO;
using Newtonsoft.Json;
using ProtobufGenerator.Interfaces;

namespace ProtobufGenerator.Configuration
{
    public class JsonConfiguration : IConfiguration
    {
        public JobSet JobSet { get; }
        public JsonConfiguration(string filePath)
        {
            using (var sr = new StreamReader(new FileStream(filePath, FileMode.Open)))
            {
                var json = sr.ReadToEnd();
                JobSet = JsonConvert.DeserializeObject<JobSet>(json);
            }
        }
    }
}
