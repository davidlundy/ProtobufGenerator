using ProtobufCompiler;
using ProtobufCompiler.Interfaces;
using ProtobufGenerator.Extensions;
using ProtobufGenerator.Generation;
using ProtobufGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtobufGenerator
{
    public class ProtoEngine : IGenerateProto
    {
        private readonly IConfiguration _config;
        private readonly IProtoCompiler _protoCompiler;
        private readonly Dictionary<string, JobResult> _jobResults = new Dictionary<string, JobResult>();

        public ProtoEngine(IConfiguration configuration, IProtoCompiler compiler = null)
        {
            _config = Check.NotNull(configuration, nameof(configuration));
            _protoCompiler = compiler ?? new ProtoCompiler();
        }

        public void ProcessProto()
        {
            foreach (var job in _config.JobSet.Jobs)
            {
                var protoDirectory = job.ProtoDirectory;
                var fileList = Directory.GetFiles(protoDirectory, "*.proto", SearchOption.AllDirectories);
                var generator = new RoslynGenerator(job);

                foreach (var file in fileList)
                {
                    var compilation = _protoCompiler.Compile(file);
                    if (compilation.Errors.Count > 0)
                    {
                        foreach(var error in compilation.Errors)
                        {
                            Console.WriteLine(error);
                        }
                        continue;
                    }
                    var descriptor = compilation.FileDescriptor;
                    var fileContent = generator.Generate(descriptor);
                    foreach(var content in fileContent)
                    {
                        File.WriteAllText(Path.Combine(protoDirectory, content.FileName), content.FileContent);
                    }
                }
            }
        }
    }
}