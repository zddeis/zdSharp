﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;

namespace zds.Core.Statements
{
    public class ReturnStatement : IStatement
    {
        public IExpression? Value { get; }

        public ReturnStatement(IExpression? value)
        {
            Value = value;
        }
    }
}