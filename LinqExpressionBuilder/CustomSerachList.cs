using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqExpressionBuilder
{
    public class CustomSearchList
    {
        public static List<SearchFilter> CustomSearchFilter<T>(T entity, Dictionary<string, Operation> customOperation = null)
        {
            List<SearchFilter> searches = new List<SearchFilter>();

            foreach (var item in PropTable<T>())
            {
                var value = typeof(T).GetProperty(item).GetValue(entity);
                if (value != null)
                {
                    searches.Add(new SearchFilter()
                    {
                        Operation = customOperation == null ? 0 : customOperation.FirstOrDefault(x => x.Key == item).Value,
                        PropertyName = item,
                        Value = value
                    });
                }
            }

            return searches;
        }
        private static List<string> PropTable<T>()
        {
            var props = typeof(T).GetProperties();
            var propTable = new List<string>();
            foreach (var prop in props)
            {
                propTable.Add(prop.Name);
            }
            return propTable;
        }

    }
}
