using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;

namespace zds.Core.Statements
{
    public class IfStatement : IStatement
    {
        public IExpression Condition { get; }
        public List<IStatement> ThenBranch { get; }
        public List<IStatement>? ElseBranch { get; }

        public IfStatement(IExpression condition, List<IStatement> thenBranch, List<IStatement>? elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }
    }
}