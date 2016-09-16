using ProtobufGenerator.Interfaces;
using System;
using System.Collections.Generic;
using ProtobufGenerator.Generation;
using ProtobufGenerator.Extensions;
using System.IO;

namespace ProtobufGenerator
{
    public class ProtoEngine : IGenerateProto
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<string, JobResult> _jobResults = new Dictionary<string, JobResult>();

        public ProtoEngine(IConfiguration configuration)
        {
            _config = Check.NotNull(configuration, nameof(configuration));
        }

        public void ProcessProto()
        {
            foreach (var job in _config.JobSet.Jobs)
            {
                var protoDirectory = job.ProtoDirectory;
                var fileList = Directory.GetFiles(protoDirectory, "*.proto", SearchOption.AllDirectories);

            }
        }
    }
}

