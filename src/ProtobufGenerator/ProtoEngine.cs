using ProtobufGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtobufGenerator.Generation;

namespace ProtobufGenerator
{
    public class ProtoEngine : IGenerateProto
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<string, JobResult> _jobResults = new Dictionary<string, JobResult>();

        public ProtoEngine(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void ProcessProto()
        {
            throw new NotImplementedException();
        }
    }
}
