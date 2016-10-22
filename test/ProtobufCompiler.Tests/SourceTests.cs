using FluentAssertions;
using ProtobufCompiler.Compiler;
using System.IO;
using System.Text;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class SourceTests
    {
        private readonly Source _sys;

        private const string Data = "L1\r\nL2\rL3\nL4";

        public SourceTests()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(Data));
            var reader = new StreamReader(memStream);
            _sys = new Source(reader);
        }

        [Fact]
        // The first 'Next' will get the first character in stream at Column 1
        public void SourceShouldStartatLine1Column0()
        {
            _sys.Line.Should().Be(1, "There is no Line 0");
            _sys.Column.Should().Be(0, "The first 'Next' will be Column 1");
        }

        [Fact]
        public void ShouldNextCharacters()
        {
            _sys.Next().ShouldBeEquivalentTo('L', "Because it is the 1st character.");
            _sys.Next().ShouldBeEquivalentTo('1', "Because it is the 2nd character.");
        }

        [Fact]
        public void NextShouldIncrementColumnCount()
        {
            _sys.Next();
            _sys.Column.Should().Be(1, "Because this is the first character read.");
            _sys.Next();
            _sys.Column.Should().Be(2, "Because we read another character after the first.");
        }

        [Fact]
        public void ShouldIncrementLineAndResetColumnonWindows()
        {
            char output;
            do
            {
                output = _sys.Next();
            } while (!output.Equals('2'));

            _sys.Line.Should().Be(2, "Because we have seeked past a CRLF on Windows");
            _sys.Column.Should().Be(2, "Because this is the 2nd character on the line.");
        }

        [Fact]
        public void ShouldIncrementLineAndResetColumnonMac()
        {
            char output;
            do
            {
                output = _sys.Next();
            } while (!output.Equals('3'));

            _sys.Line.Should().Be(3, "Because we have seeked past a CR on Mac");
            _sys.Column.Should().Be(2, "Because this is the 2nd character on the line.");
        }

        [Fact]
        public void ShouldIncrementLineAndResetColumnonLinux()
        {
            char output;
            do
            {
                output = _sys.Next();
            } while (!output.Equals('4'));

            _sys.Line.Should().Be(4, "Because we have seeked past a LF on Linux");
            _sys.Column.Should().Be(2, "Because this is the 2nd character on the line.");
        }

        [Fact]
        public void ShouldReadToEnd()
        {
            while (!_sys.EndStream)
            {
                _sys.Next();
            }

            _sys.EndStream.Should().BeTrue("We have seeked to the end of the stream");
            _sys.Line.Should().Be(4, "Because we have seeked past a LF on Linux");
            _sys.Column.Should().Be(2, "Because this is the 2nd character on the line.");

            _sys.Invoking(s => s.Next()).ShouldThrow<EndOfStreamException>();
        }
    }
}