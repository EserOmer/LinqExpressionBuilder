using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqExpressionBuilder
{
    public static class ExpressionBuilder
    {
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains",
            new[] { typeof(string) });

        private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith",
            new[] { typeof(string) });

        private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith",
            new[] { typeof(string) });

        public static Expression<Func<T, bool>> GetExpression<T>(IList<SearchFilter> filters)
        {
            if (filters.Count == 0)
                return null;

            var param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpression<T>(param, filters[0]);
            else if (filters.Count == 2)
                exp = GetExpression<T>(param, filters[0], filters[1]);
            else
            {
                while (filters.Count > 0)
                {
                    var f1 = filters[0];
                    var f2 = filters[1];

                    exp = exp == null ?
                        GetExpression<T>(param, filters[0], filters[1]) :
                        Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        private static Expression GetExpression<T>(ParameterExpression param, SearchFilter filter)
        {
            var member = Expression.Property(param, filter.PropertyName);

            var constant = Expression.Constant(filter.Value, member.Type);

            switch (filter.Operation)
            {
                case Operation.Equals:
                    return Expression.Equal(member, constant);

                case Operation.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case Operation.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case Operation.LessThan:
                    return Expression.LessThan(member, constant);

                case Operation.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);

                case Operation.Contains:
                    return Expression.Call(member, ContainsMethod, constant);

                case Operation.StartsWith:
                    return Expression.Call(member, StartsWithMethod, constant);

                case Operation.EndsWith:
                    return Expression.Call(member, EndsWithMethod, constant);
            }

            return null;
        }

        private static BinaryExpression GetExpression<T>(ParameterExpression param, SearchFilter filter1,
            SearchFilter filter2)
        {
            var bin1 = GetExpression<T>(param, filter1);
            var bin2 = GetExpression<T>(param, filter2);

            return Expression.AndAlso(bin1, bin2);
        }
    }
}
