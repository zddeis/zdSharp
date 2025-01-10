using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;

namespace zds.Core.Statements
{
    public class ExpressionStatement : IStatement
    {
        public IExpression Expression { get; }

        public ExpressionStatement(IExpression expression)
        {
            Expression = expression;
        }
    }
}