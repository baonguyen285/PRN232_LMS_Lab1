using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PRN232.LMS.Services.Common
{
    public static class QueryHelper
    {
        public static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string sortExpression)
        {
            if (string.IsNullOrWhiteSpace(sortExpression)) return query;

            var sortParts = sortExpression.Split(',', StringSplitOptions.RemoveEmptyEntries);
            bool isFirst = true;

            foreach (var part in sortParts)
            {
                var trimmed = part.Trim();
                bool descending = trimmed.StartsWith("-");
                var propertyName = descending ? trimmed.Substring(1) : trimmed;

                query = ApplyOrder(query, propertyName, descending, isFirst);
                isFirst = false;
            }

            return query;
        }

        private static IQueryable<T> ApplyOrder<T>(IQueryable<T> query, string propertyName, bool descending, bool isFirst)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            
            // Handle property with case insensitivity
            var propInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                     .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            
            if (propInfo == null) return query; // Skip if property not found

            var propertyAccess = Expression.MakeMemberAccess(parameter, propInfo);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            string methodName = isFirst 
                ? (descending ? "OrderByDescending" : "OrderBy") 
                : (descending ? "ThenByDescending" : "ThenBy");

            var resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), propInfo.PropertyType },
                query.Expression,
                Expression.Quote(orderByExp)
            );

            return query.Provider.CreateQuery<T>(resultExp);
        }

        public static object FilterFields<T>(T obj, string fields)
        {
            if (obj == null) return null!;
            if (string.IsNullOrWhiteSpace(fields)) return obj;

            var fieldsList = fields.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(f => f.Trim())
                                   .ToList();

            var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (fieldsList.Any(f => f.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    dict[prop.Name] = prop.GetValue(obj);
                }
            }

            return dict;
        }

        public static List<object> FilterFieldsList<T>(IEnumerable<T> list, string fields)
        {
            var result = new List<object>();
            if (list == null) return result;

            foreach (var item in list)
            {
                result.Add(FilterFields(item, fields));
            }
            return result;
        }
    }
}

