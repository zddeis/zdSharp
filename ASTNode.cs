using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zdSharp
{
    public abstract class ASTNode
    {
        public abstract void Accept(IVisitor visitor);
    }

    // Block of Statements
    public class BlockNode : ASTNode
    {
        public List<ASTNode> Statements { get; }
        
        public BlockNode(List<ASTNode> statements)
        {
            Statements = statements;
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    // Variable Assignment
    public class AssignmentNode : ASTNode
    {
        public string VariableName { get; }
        public ASTNode Value { get; }

        public AssignmentNode(string variableName, ASTNode value)
        {
            VariableName = variableName;
            Value = value;
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    // Print
    public class PrintNode : ASTNode
    {
        public ASTNode Expression { get; }

        public PrintNode(List<ASTNode> args)
        {
            Expression = args[0];
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    // Print
    public class WaitKeyNode : ASTNode
    {
        public string Key;

        public WaitKeyNode(List<ASTNode> args)
        {

        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    // Binary
    public class BinaryExpressionNode : ASTNode
    {
        public ASTNode Left { get; }
        public string Operator { get; }
        public ASTNode Right { get; }

        public BinaryExpressionNode(ASTNode left, string operatorSymbol, ASTNode right)
        {
            Left = left;
            Operator = operatorSymbol;
            Right = right;
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    // Variable
    public class VariableNode : ASTNode
    {
        public string Name { get; }

        public VariableNode(string name)
        {
            Name = name;
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    // Number
    public class NumberNode : ASTNode
    {
        public double Value { get; }

        public NumberNode(double value)
        {
            Value = value;
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    // String
    public class StringNode : ASTNode
    {
        public string Value { get; }

        public StringNode(string value)
        {
            Value = value;
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
