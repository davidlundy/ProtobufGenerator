using ProtobufCompiler.Compiler;
using ProtobufCompiler.Compiler.Types;
using ProtobufCompiler.Interfaces;
using System.IO;

namespace ProtobufCompiler
{
    public class ProtoCompiler : IProtoCompiler
    {
        public Compilation Compile(string filePath)
        {
            var streamReader = File.OpenText(filePath);
            var compileSource = new Source(streamReader);
            var lexicalAnalyzer = new LexicalAnalyzer(compileSource);

            lexicalAnalyzer.Tokenize();

            var syntaxAnalyzer = new RootSyntaxAnalyzer();

            var nodeResult = syntaxAnalyzer.Analyze(lexicalAnalyzer.TokenStream);

            var builderVisitor = new BuilderVisitor();
            builderVisitor.Visit(nodeResult.Node);

            return new Compilation
            {
                FileDescriptor = builderVisitor.FileDescriptor,
                Errors = (System.Collections.Generic.ICollection<Compiler.Errors.CompilerError>)nodeResult.Errors
            };
        }
    }
}