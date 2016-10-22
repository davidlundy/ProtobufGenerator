using FluentAssertions;
using ProtobufCompiler.Compiler.Types;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class TypeTests
    {
        [Theory]
        [InlineData("double", SimpleType.Double)]
        [InlineData("float", SimpleType.Float)]
        [InlineData("int32", SimpleType.Int32)]
        [InlineData("int64", SimpleType.Int64)]
        [InlineData("uint32", SimpleType.Uint32)]
        [InlineData("uint64", SimpleType.Uint64)]
        [InlineData("sint32", SimpleType.Sint32)]
        [InlineData("sint64", SimpleType.Sint64)]
        [InlineData("fixed32", SimpleType.Fixed32)]
        [InlineData("fixed64", SimpleType.Fixed64)]
        [InlineData("sfixed32", SimpleType.Sfixed32)]
        [InlineData("sfixed64", SimpleType.Sfixed64)]
        [InlineData("bool", SimpleType.Bool)]
        [InlineData("string", SimpleType.String)]
        [InlineData("bytes", SimpleType.Bytes)]
        [InlineData("george", SimpleType.None)]
        [InlineData("BOOL", SimpleType.None)]
        public void FieldShouldCreateSimpleTypes(string fieldType, SimpleType expected)
        {
            var field = new Field(fieldType, "na", 1, null, false);
            field.SimpleType.Should().Be(expected);
        }
    }
}