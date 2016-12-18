using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class PackageAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var errors = new List<ParseError>();

            var packageTag = tokens.Dequeue();
            var packageNode = new Node(NodeType.Package, packageTag.Lexeme);

            var packageName = ParseFullIdentifier(tokens, nameof(PackageAnalyzer));

            if (packageName.Errors.Any()) return packageName;

            packageNode.AddChild(packageName.Node);

            if (!HasTerminator(tokens))
            {
                tokens.BurnLine();
                return new NodeResult<Node>(null, new[] { new ParseError("Required terminator is missing for statement. Found token ", packageTag) });
            }

            tokens.Dequeue(); // The terminator token; i.e. ';'

            tokens.DumpEndline();

            return new NodeResult<Node>(packageNode);
        }
    }
}
