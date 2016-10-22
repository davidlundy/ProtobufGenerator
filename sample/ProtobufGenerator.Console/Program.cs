using ProtobufGenerator.Configuration;
using System;
using System.Collections.Generic;

namespace ProtobufGenerator.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var engine = new ProtoEngine(new JsonConfiguration(new JobSet
            {
                SolutionDirectory = AppContext.BaseDirectory,
                Jobs = new List<Job>
                {
                    new Job
                    {
                        Name = "test job",
                        ProtoDirectory = AppContext.BaseDirectory,
                        OutputDirectory = AppContext.BaseDirectory
                    }
                }
            }));

            engine.ProcessProto();
        }
    }
}