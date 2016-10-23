using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using ProtobufCompiler.Compiler.Types;
using ProtobufGenerator.Configuration;
using ProtobufGenerator.Interfaces;
using System.Collections.Generic;

namespace ProtobufGenerator.Generation
{
    public class RoslynGenerator : IRoslynGenerator
    {
        private AdhocWorkspace _workspace;
        private readonly SyntaxGenerator _generator;
        private string _namespace;
        private IDictionary<string, IList<string>> _customAnnotations;
        private IEnumerable<string> _nullableClasses;

        public RoslynGenerator(Job job)
        {
            _namespace = job.DestinationNamespace;
            _customAnnotations = job.CustomAnnotations;
            _nullableClasses = job.NullableClasses;

            _workspace = new AdhocWorkspace();
            _generator = SyntaxGenerator.GetGenerator(_workspace, LanguageNames.CSharp);
        }

        public ICollection<JobResult> Generate(FileDescriptor descriptor)
        {
            var results = new List<JobResult>();

            foreach (var field in descriptor.Messages)
            {
                results.Add(GenerateClass(field, descriptor.Package, descriptor.Imports));
            }

            return results;
        }

        private JobResult GenerateClass(MessageDefinition msgDef, Package package, ICollection<Import> imports)
        {
            var topLevelStatements = new List<SyntaxNode>();
            topLevelStatements.Add(_generator.NamespaceImportDeclaration("System"));
            foreach(var import in imports)
            {
                topLevelStatements.Add(_generator.NamespaceImportDeclaration(import.ImportClass));
            }

            var members = new List<SyntaxNode>();
            foreach (var msgField in msgDef.Fields)
            {
                members.Add(_generator.FieldDeclaration(
                    $"_{msgField.FieldName}",
                    _generator.TypeExpression(SpecialType.System_String),
                    Accessibility.Private));
                members.Add(_generator.PropertyDeclaration(
                    $"{msgField.FieldName}",
                    _generator.TypeExpression(SpecialType.System_String),
                    Accessibility.Public,
                    getAccessorStatements: new SyntaxNode[]
                    {
                        _generator.ReturnStatement(_generator.IdentifierName($"_{msgField.FieldName}"))
                    },
                    setAccessorStatements: new SyntaxNode[]
                    {
                        _generator.AssignmentStatement(_generator.IdentifierName($"_{msgField.FieldName}"), _generator.IdentifierName("value"))
                    }));
            }

            var classDefinition = _generator.ClassDeclaration(msgDef.Name,
                typeParameters: null,
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Sealed,
                baseType: null,
                interfaceTypes: null,
                members: members);

            var packageName = package?.Name ?? "wasnull";

            var ns = _generator.NamespaceDeclaration(packageName, classDefinition);

            topLevelStatements.Add(ns);

            var compilationUnit = _generator.CompilationUnit(topLevelStatements)
                .NormalizeWhitespace();

            return new JobResult
            {
                FileContent = compilationUnit.ToFullString(),
                FileName = $"{msgDef.Name}.cs"
            };
        }
    }
}