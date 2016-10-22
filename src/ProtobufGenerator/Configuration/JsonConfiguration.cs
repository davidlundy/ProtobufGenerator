using Newtonsoft.Json;
using ProtobufGenerator.Interfaces;
using System.IO;

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

        public JsonConfiguration(JobSet jobSet)
        {
            JobSet = jobSet;
        }
    }
}