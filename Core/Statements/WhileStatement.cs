using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;

namespace zds.Core.Statements
{
    public class WhileStatement : IStatement
    {
        public IExpression Condition { get; }
        public List<IStatement> Body { get; }

        public WhileStatement(IExpression condition, List<IStatement> body)
        {
            Condition = condition;
            Body = body;
        }
    }
}