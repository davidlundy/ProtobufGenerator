using ProtobufCompiler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtobufCompiler.Compiler.Types;
using System.IO;
using ProtobufCompiler.Compiler;

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

            var syntaxAnalyzer = new SyntaxAnalyzer(lexicalAnalyzer.TokenStream);

            var rootNode = syntaxAnalyzer.Analyze();

            var builderVisitor = new BuilderVisitor();
            builderVisitor.Visit(rootNode);

            return new Compilation
            {
                FileDescriptor = builderVisitor.FileDescriptor,
                Errors = builderVisitor.Errors
            };
        }
    }
}
