using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqExpressionBuilder
{
    public class ExpressionBuilderHelper
    {
        public static List<SearchFilter> ChangeFilterValueType<T>(List<SearchFilter> searchFilters)
        {
            var propType = typeof(T).GetProperties();
            foreach (var item in searchFilters)
            {
                item.Value = Convert.ChangeType(item.Value.ToString(), propType.Where(p => p.Name == item.PropertyName).FirstOrDefault().PropertyType);
            }
            return searchFilters;
        }
        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var body = propertyLambda.Body as MemberExpression;
            return body.Member.Name;
        }
    }
}
