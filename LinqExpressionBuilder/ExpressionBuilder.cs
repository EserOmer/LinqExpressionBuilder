using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqExpressionBuilder
{
    public static class ExpressionBuilder
    {
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        public static Expression<Func<T, bool>> GetExpression<T>(IList<SearchFilter> filters)
        {
            if (filters == null || filters.Count == 0)
                return null;

            var param = Expression.Parameter(typeof(T), "t");
            Expression exp = GetExpression<T>(param, filters[0]);

            for (int i = 1; i < filters.Count; i++)
            {
                var next = GetExpression<T>(param, filters[i]);
                exp = filters[i].Connector == Connector.Or
                    ? Expression.OrElse(exp, next)
                    : Expression.AndAlso(exp, next);
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

                case Operation.NotEquals:
                    return Expression.NotEqual(member, constant);

                case Operation.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case Operation.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case Operation.LessThan:
                    return Expression.LessThan(member, constant);

                case Operation.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);

                case Operation.Contains:
                    EnsureStringProperty(filter.PropertyName, member.Type);
                    return Expression.Call(member, ContainsMethod, constant);

                case Operation.StartsWith:
                    EnsureStringProperty(filter.PropertyName, member.Type);
                    return Expression.Call(member, StartsWithMethod, constant);

                case Operation.EndsWith:
                    EnsureStringProperty(filter.PropertyName, member.Type);
                    return Expression.Call(member, EndsWithMethod, constant);

                default:
                    throw new NotSupportedException($"Operation '{filter.Operation}' is not supported.");
            }
        }

        private static void EnsureStringProperty(string propertyName, Type type)
        {
            if (type != typeof(string))
                throw new InvalidOperationException($"Operation requires a string property, but '{propertyName}' is of type '{type.Name}'.");
        }
    }
}
