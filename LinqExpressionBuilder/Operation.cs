using System;
using System.Collections.Generic;
using System.Text;

namespace LinqExpressionBuilder
{
    public enum Operation
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }

    public enum Connector
    {
        And,
        Or
    }
}
