using System;
using System.Collections.Generic;
using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;
using ProtobufCompiler.Compiler.Types;
using System.Linq;

namespace ProtobufCompiler.Compiler.Types
{
    internal class BuilderVisitor : IVisitor
    {
        public FileDescriptor FileDescriptor { get; internal set; } = new FileDescriptor();

        public ICollection<ParseError> Errors { get; internal set; } = new List<ParseError>();

        // ReSharper disable once EmptyConstructor : Prevent external construction
        internal BuilderVisitor()
        { 
            
        }

        public void Visit(Node node)
        {
            if (!node.NodeType.Equals(NodeType.Root))
                throw new InvalidOperationException("Cannot use BuilderVisitor on non-root Node");

            var syntaxNode = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.Syntax));
            var syntaxVisitor = new SyntaxVisitor(Errors);
            syntaxNode?.Accept(syntaxVisitor);
            FileDescriptor.Syntax = syntaxVisitor.Syntax;

            var packageNode = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.Package));
            var packageVisitor = new PackageVisitor(Errors);
            packageNode?.Accept(packageVisitor);
            FileDescriptor.Package = packageVisitor.Package;

            var optionNodes = node.Children.Where(t => t.NodeType.Equals(NodeType.Option));
            foreach (var option in optionNodes)
            {
                var optionVisitor = new OptionVisitor(Errors);
                option.Accept(optionVisitor);
                FileDescriptor.Options.Add(optionVisitor.Option);
            }

            var importNodes = node.Children.Where(t => t.NodeType.Equals(NodeType.Import));
            foreach (var import in importNodes)
            {
                var importVisitor = new ImportVisitor(Errors);
                import.Accept(importVisitor);
                FileDescriptor.Imports.Add(importVisitor.Import);
            }

            var messageNodes = node.Children.Where(t => t.NodeType.Equals(NodeType.Message));
            foreach (var option in messageNodes)
            {
                var messageVisitor = new MessageVisitor(Errors);
                option.Accept(messageVisitor);
                FileDescriptor.Messages.Add(messageVisitor.Message);
            }

            var enumNodes = node.Children.Where(t => t.NodeType.Equals(NodeType.Enum));
            foreach (var option in enumNodes)
            {
                var enumVisitor = new EnumVisitor(Errors);
                option.Accept(enumVisitor);
                FileDescriptor.Enumerations.Add(enumVisitor.EnumDefinition);
            }

            var serviceNodes = node.Children.Where(t => t.NodeType.Equals(NodeType.Service));
            foreach (var service in serviceNodes)
            {
                var serviceVisitor = new ServiceVisitor(Errors);
                service.Accept(serviceVisitor);
                FileDescriptor.Services.Add(serviceVisitor.Service);
            }
        }
    }
}
