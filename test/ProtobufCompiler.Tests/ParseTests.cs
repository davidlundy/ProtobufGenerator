using System;
using System.Collections.Generic;
using Xunit;
using ProtobufCompiler.Types;
using Sprache;

namespace ProtobufCompiler.Tests
{
    public class ParseTests
    {
        private readonly ProtoGrammar _sys;

        public ParseTests()
        {
            _sys = new ProtoGrammar();
        }

        [Theory]
        [InlineData("syntax = \"proto3\";", "proto3")]
        [InlineData("syntax = \"proto2\";", "proto2")]
        // syntax = "syntax" "=" quote "proto3" quote ";"
        public void Syntax_Declaration(string input, string expected)
        {
            var syntax = _sys.Syntax.Parse(input);
            Assert.Equal(syntax, new Syntax(expected));
        }

        [Theory]
        [InlineData("syntax \"proto\";")]
        // syntax = "syntax" "=" quote "proto3" quote ";"
        public void Syntax_Declaration_Throws_on_BadGrammar(string input)
        {
            Assert.Throws<ParseException>(() => _sys.Syntax.Parse(input));
        }

        [Theory]
        [InlineData("syntax = \"proto\";")]
        // syntax = "syntax" "=" quote "proto3" quote ";"
        public void Syntax_Declaration_Throws_Argument_on_BadSyntax(string input)
        {
            Assert.Throws<ArgumentException>(() => _sys.Syntax.Parse(input));
        }

        [Theory]
        [InlineData("import public \"Test.Proto.OtherClass\";", "public", "Test.Proto.OtherClass")]
        [InlineData("import \"Test.Proto.OtherClass\";", "", "Test.Proto.OtherClass")]
        [InlineData("import \"Test.Proto.OtherClass\";", "None", "Test.Proto.OtherClass")]
        [InlineData("import weak \"Test.Proto.OtherClass\";", "Weak", "Test.Proto.OtherClass")]
        //import = "import" [ "weak" | “public”] strLit ";" 
        public void Import_Declaration(string input, string expMod, string expValue)
        {
            Assert.Equal(new Import(expMod, expValue), _sys.Import.Parse(input));
        }

        [Theory]
        [InlineData("import weak Test.Proto.OtherClass;")]
        [InlineData("import strong Test.Proto.OtherClass;")]
        //import = "import" [ "weak" | “public”] strLit ";" 
        public void Import_Declaration_Throwswhen_NotQuoted_or_BadMod(string input)
        {
            Assert.Throws<ParseException>(() => _sys.Import.Parse(input));
        }

        [Theory]
        [InlineData("package Test.Proto.Classes;", "Test.Proto.Classes")]
        // package = "package" fullIdent ";"
        public void Package_Declaration(string input, string expected)
        {
            Assert.Equal(new Package(expected), _sys.Package.Parse(input));
        }

        [Theory]
        [InlineData("option SOME_KEY = SOME_VALUE;", "SOME_KEY", "SOME_VALUE")]
        [InlineData("option SOME_KEY = \"SOME_VALUE.SOME_VALUE\";", "SOME_KEY", "SOME_VALUE.SOME_VALUE")]
        // option = "option" optionName  "=" constant ";"
        // optionName = (ident | "(" fullIdent ")") {"." ident}
        public void Option_Declaration(string input, string expKey, string expValue)
        {
            Assert.Equal(new Option(expKey, expValue), _sys.Option.Parse(input));
        }

        [Theory, MemberData("Fields")]
        internal void Field_Declaration(string input, Field field)
        {
            Assert.Equal(field, _sys.Field.Parse(input));
        }

        public static IEnumerable<object[]> Fields
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        "foo.bar nested_message = 2;",
                        new Field("foo.bar", "nested_message", 2, new List<Option>(), false)
                    },
                    new object[]
                    {
                        "repeated int32 samples = 4;",
                        new Field("int32", "samples", 4, new List<Option>(), true)
                    }
                };
            }
        }

        [Theory, MemberData("OneOfFields")]
        internal void OneOfField_Declaration(string input, OneOfField field)
        {
            if (input.Contains("repeated"))
            {
                Assert.Throws<ParseException>(() => _sys.OneOfField.Parse(input));
            }
            else
            {
                Assert.Equal(field, _sys.OneOfField.Parse(input));
            }
        }

        public static IEnumerable<object[]> OneOfFields
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        "foo.bar nested_message = 2;",
                        new OneOfField("foo.bar", "nested_message", 2, new List<Option>())
                    },
                    new object[]
                    {
                        "repeated int32 samples = 4;",
                        new OneOfField("int32", "samples", 4, new List<Option>())
                    }
                };
            }
        }
            
            
        [Theory, MemberData("OneOf")]
        internal void OneOf_Declaration(string input, OneOf oneOf)
        {
            Assert.Equal(oneOf, _sys.OneOf.Parse(input));
        }

        public static IEnumerable<object[]> OneOf
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        @"oneof foo {
                            string name = 4;
                            SomeType some_type = 9;
                          }",
                        new OneOf("foo", new[]
                        {
                            new OneOfField("string", "name", 4, new List<Option>()),
                            new OneOfField("SomeType", "some_type", 9, new List<Option>())
                        })
                    }
                };
            }
        }

        [Theory, MemberData("MapData")]
        internal void Map_Declaration(string input, Map map)
        {
            Assert.Equal(map, _sys.Map.Parse(input));
        }

        public static IEnumerable<object[]> MapData
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        @"map<string, Project> projects = 3;",
                        new Map("projects", 3, "string", "Project")
                    }
                };
            }
        }

        [Theory, MemberData("NameReservations")]
        internal void ReserveNames_Declaration(string input, IEnumerable<string> res)
        {
            Assert.Equal(res, _sys.NameReservation.Parse(input));
        }

        public static IEnumerable<object[]> NameReservations
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        "reserved \"foo\", \"bar\";",
                        new List<string> {"foo", "bar" }
                    }
                };
            }
        }

        [Theory, MemberData("NumberReservations")]
        internal void ReserveNumber_Declaration(string input, IEnumerable<int> res)
        {
            Assert.Equal(res, _sys.NumberReservation.Parse(input));
        }

        public static IEnumerable<object[]> NumberReservations
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        "reserved 2, 15, 9 to 11;",
                        new List<int> {2, 15, 9, 10, 11 }
                    },
                    new object[]
                    {
                        "reserved 17 to 20, 2, 15, 9 to 11;",
                        new List<int> {17, 18, 19, 20, 2, 15, 9, 10, 11 }
                    }
                };
            }
        }
    }
}
