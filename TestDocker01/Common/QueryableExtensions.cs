using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestDocker01.Common
{
    /// <summary>
    /// This class contains IQueryable<T> extension methods used in this project.
    /// </summary>
    internal static class QueryableExtensions
    {
        /// <summary>
        /// Applies OrderBy extension method to the given Queryable source.
        /// </summary>
        /// <typeparam name="T">The type of queried entities.</typeparam>
        /// <param name="source">The Queryable source.</param>
        /// <param name="ordering">The order column name.</param>
        /// <returns>The new Queryable with applied OrderBy.</returns>
        /// <remarks>Thrown exceptions will be propagated.</remarks>
        internal static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering)
        {
            return OrderbyFromColumnName(source, "OrderBy", ordering);
        }

        /// <summary>
        /// Applies OrderByDescending extension method to the given Queryable source.
        /// </summary>
        /// <typeparam name="T">The type of queried entities.</typeparam>
        /// <param name="source">The Queryable source.</param>
        /// <param name="ordering">The order column name.</param>
        /// <returns>The new Queryable with applied OrderByDescending.</returns>
        /// <remarks>Thrown exceptions will be propagated.</remarks>
        internal static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source,
            string ordering)
        {
            return OrderbyFromColumnName(source, "OrderByDescending", ordering);
        }

        /// <summary>
        /// Use Expression to add extension method to IQueryable.
        /// So it support order by nested property name.
        /// </summary>
        /// <typeparam name="T">The type of queried entities.</typeparam>
        /// <param name="source">The Queryable source.</param>
        /// <param name="orderName">The order name.</param>
        /// <param name="colName">The column name.</param>
        /// <returns>The new IQueryable with applied ordering.</returns>
        /// <remarks>Thrown exceptions will be propagated.</remarks>
        private static IQueryable<T> OrderbyFromColumnName<T>(IQueryable<T> source, string orderName, string colName)
        {
            var props = colName.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                var pi = type.GetPublicProperties().FirstOrDefault(
                    p => p.Name.Equals(prop, StringComparison.OrdinalIgnoreCase));
                if (pi == null)
                {
                    throw new ArgumentException($"'{colName}' is not a valid SortBy value.");
                }
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);
            var resultExp = Expression.Call(typeof(Queryable),
                orderName, new[] { typeof(T), type }, source.Expression, lambda);
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// Gets all public properties of the given type.
        /// </summary>
        /// <param name="type">The type to get properties for.</param>
        /// <returns>All public properties of the given type.</returns>
        private static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if (!type.IsInterface)
            {
                return type.GetProperties();
            }

            return (new Type[] { type }).Concat(type.GetInterfaces()).SelectMany(i => i.GetProperties());
        }
    }
}
