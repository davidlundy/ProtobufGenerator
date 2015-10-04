using System;
using System.Collections.Generic;
using ProtobufCompiler.Types;
using Sprache;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class DefinitionTests
    {
        private readonly ProtoGrammar _sys;

        public DefinitionTests()
        {
            _sys = new ProtoGrammar();
        }

        [Theory, MemberData("EnumData")]
        internal void Enum_Definition_Test(string input, EnumDefinition def)
        {
            Assert.Equal(def, _sys.EnumDefinition.Parse(input));
        }

        public static IEnumerable<object[]> EnumData
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        @"enum EnumAllowingAlias {"+Environment.NewLine+
                            @"option allow_alias = true;"+Environment.NewLine+
                            @"UNKNOWN = 0;"+Environment.NewLine+
                            @"STARTED = 1;"+Environment.NewLine+
                            @"RUNNING = 2 [(custom_option) = ""hello world""];"+Environment.NewLine+
                        @"}",
                        new EnumDefinition("EnumAllowingAlias", 
                            new List<Option> {new Option("allow_alias", "true")},
                            new List<EnumField>()
                            {
                                new EnumField("UNKNOWN", 0, null),
                                new EnumField("STARTED", 1, null),
                                new EnumField("RUNNING", 2, new List<Option> { new Option("custom_option", "hello world")} )
                            })
                    }
                };
            }
        }

        [Theory, MemberData("ServiceData")]
        internal void Service_Definition_Test(string input, ServiceDefinition def)
        {
            Assert.Equal(def, _sys.ServiceDefinition.Parse(input));
        }

        public static IEnumerable<object[]> ServiceData
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        @"service SearchService {"+Environment.NewLine+
                            @"option allow_alias = true;"+Environment.NewLine+
                            @"rpc Search ([streaming] SearchRequest) returns ([streaming] SearchResponse);"+Environment.NewLine+
                        @"}",
                        new ServiceDefinition("SearchService", 
                            new List<ServiceMethod>
                            {
                                new ServiceMethod("Search", 
                                    new ParameterType("SearchRequest", true),
                                    new ParameterType("SearchResponse", true))
                            },
                            new List<Option>
                            {
                                new Option("allow_alias", "true")
                            }
                        ), 
                    }
                };
            }
        }
    }
}
