using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public interface IExpression
    {
        object? Evaluate();
        int Line { get; }
    }
}