using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                            new Option("allow_alias", "true"),
                            new List<EnumField>()
                            {
                                new EnumField("UNKNOWN", 0, null),
                                new EnumField("STARTED", 1, null),
                                new EnumField("RUNNING", 2, new Option("custom_option", "hello world"))
                            })
                    }
                };
            }
        }
    }
}
