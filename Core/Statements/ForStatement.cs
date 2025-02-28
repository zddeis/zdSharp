using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;

namespace zds.Core.Statements
{
    public class ForStatement : IStatement
    {
        public string Variable { get; }
        public IExpression Start { get; }
        public IExpression End { get; }
        public IExpression? Step { get; }
        public List<IStatement> Body { get; }

        public ForStatement(string variable, IExpression start, IExpression end, IExpression? step, List<IStatement> body)
        {
            Variable = variable;
            Start = start;
            End = end;
            Step = step;
            Body = body;
        }
    }
}