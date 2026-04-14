using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqExpressionBuilder
{
    public static class ExpressionBuilderHelper
    {
        public static List<SearchFilter> ChangeFilterValueType<T>(List<SearchFilter> searchFilters)
        {
            if (searchFilters == null) throw new ArgumentNullException(nameof(searchFilters));

            var propTypes = typeof(T).GetProperties();
            foreach (var item in searchFilters)
            {
                var prop = propTypes.FirstOrDefault(p => p.Name == item.PropertyName)
                    ?? throw new ArgumentException($"Property '{item.PropertyName}' not found on type '{typeof(T).Name}'.");

                item.Value = Convert.ChangeType(item.Value?.ToString(), prop.PropertyType);
            }
            return searchFilters;
        }

        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            if (propertyLambda.Body is not MemberExpression body)
                throw new ArgumentException("Expression must be a member expression.", nameof(propertyLambda));

            return body.Member.Name;
        }
    }
}
