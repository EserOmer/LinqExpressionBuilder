using System.Collections.Generic;
using System.Linq;

namespace LinqExpressionBuilder
{
    public static class CustomSearchList
    {
        public static List<SearchFilter> CustomSearchFilter<T>(T entity, Dictionary<string, Operation> customOperation = null)
        {
            var searches = new List<SearchFilter>();

            foreach (var propName in typeof(T).GetProperties().Select(p => p.Name))
            {
                var value = typeof(T).GetProperty(propName)?.GetValue(entity);
                if (value == null) continue;

                customOperation?.TryGetValue(propName, out var op);
                searches.Add(new SearchFilter
                {
                    Operation = op,
                    PropertyName = propName,
                    Value = value
                });
            }

            return searches;
        }
    }
}
