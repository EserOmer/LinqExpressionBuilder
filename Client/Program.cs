using LinqExpressionBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SomeEntity entity = new SomeEntity() { ConstainProperty = "co", EndsWithProperty = "e" ,EqualProperty=1,StartWihtProperty="St"};

            Console.WriteLine(ExpressionBuilderDemo(entity).ToString());
        }
        public static Expression<Func<SomeEntity,bool>> ExpressionBuilderDemo(SomeEntity entity)
        {
            var op = new Dictionary<string, Operation>()
            {
                {
                    ExpressionBuilderHelper.GetPropertyName(()=>entity.ConstainProperty),
                    Operation.Contains
                },
                {
                    ExpressionBuilderHelper.GetPropertyName(()=>entity.StartWihtProperty),
                    Operation.StartsWith
                },
                {
                    ExpressionBuilderHelper.GetPropertyName(()=>entity.EndsWithProperty),
                    Operation.EndsWith
                },
                {
                    ExpressionBuilderHelper.GetPropertyName(()=>entity.EqualProperty),
                    Operation.Equals
                }
            };

            var customSearchFilter = CustomSearchList.CustomSearchFilter(entity, op);
            var lambda = ExpressionBuilder.GetExpression<SomeEntity>(ExpressionBuilderHelper.ChangeFilterValueType<SomeEntity>(customSearchFilter));
            return lambda;
        }
    }
}
