using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zdSharp
{
    public interface IVisitor
    {
        void Visit(BlockNode node);
        void Visit(AssignmentNode node);
        void Visit(PrintNode node);
        void Visit(WaitKeyNode node);
        void Visit(BinaryExpressionNode node);
        void Visit(VariableNode node);
        void Visit(NumberNode node);
        void Visit(StringNode node);
    }
}
