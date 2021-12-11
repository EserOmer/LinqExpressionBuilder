using System;
using System.Collections.Generic;
using System.Text;

namespace LinqExpressionBuilder
{
    public class SearchFilter
    {
        public string PropertyName { get; set; }
        public Operation Operation { get; set; }
        public object Value { get; set; }
    }
}
