using System;
using System.Collections.Generic;
using System.Text;
using ProtobufCompiler.Interfaces;

namespace ProtobufCompiler.Compiler.Nodes
{
    internal class DebugVisitor : IVisitor
    {
        private readonly Dictionary<Guid, int> _depthMonitor = new Dictionary<Guid, int>();
        private readonly StringBuilder _sb = new StringBuilder();
        private int _currentDepth;
        
        public void Visit(Node node)
        {
            if (node.NodeType.Equals(NodeType.Root))
            {
                _depthMonitor.Add(node.Guid, _currentDepth);
            }
            else
            {
                int parentDepth;
                if (_depthMonitor.TryGetValue(node.Parent.Guid, out parentDepth))
                {
                    _currentDepth = parentDepth + 1;
                    _depthMonitor.Add(node.Guid, _currentDepth);
                }
            }
            var leader = new string('-', _currentDepth);
            _sb.AppendLine(string.Format("{0} {1} : {2}", leader, node.NodeType, node.NodeValue));
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
