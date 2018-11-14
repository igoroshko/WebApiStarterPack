using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using TestDocker01.Data.Entities;
using TestDocker01.Models;

namespace TestDocker01.Common
{
    /// <summary>
    /// This class contains validation methods.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Represents the JSON serializer settings.
        /// </summary>
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = "MM/dd/yyyy HH:mm:ss",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc            
        };

        public static readonly JsonSerializerSettings SerializerWithEnumConverter = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = "MM/dd/yyyy HH:mm:ss",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = true } }
        };

        /// <summary>
        /// Checks whether the given search criteria is <c>null</c> or incorrect.
        /// </summary>
        ///
        /// <param name="criteria">The search criteria to check.</param>
        ///
        /// <exception cref="ArgumentNullException">If the <paramref name="criteria"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the <paramref name="criteria"/> is incorrect,
        /// e.g. PageNumber is negative, or PageNumber is positive and PageSize is not positive.</exception>
        public static void CheckSearchCriteria(BaseSearchCriteria criteria)
        {
            ValidateArgumentNotNull(criteria, nameof(criteria));

            if (criteria.PageIndex < 0)
            {
                throw new ArgumentException("Page number can't be negative.", nameof(criteria));
            }

            if (criteria.PageIndex > 0 && criteria.PageSize < 1)
            {
                throw new ArgumentException("Page size should be positive, if page number is positive.",
                    nameof(criteria));
            }
        }

        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    action(item);
                }
            }
        }

        public static IEnumerable<T> AsNullable<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }

        ///// <summary>
        ///// Checks whether the found entity is not null.
        ///// </summary>
        ///// <typeparam name="T">The type of the entity.</typeparam>
        ///// <typeparam name="T">The type of property used to search entity.</typeparam>
        ///// <param name="entity">The found entity.</param>
        ///// <param name="key">The key used to search entity.</param>
        ///// <param name="keyName">The name of the property used to search entity.</param>
        ///// <exception cref="EntityNotFoundException">If <paramref name="entity"/> is null.</exception>
        //public static void CheckFoundEntity<T, TKey>(T entity, TKey key, string keyName = "Id")
        //    where T : IdentifiableEntity
        //{
        //    if (entity == null)
        //    {
        //        throw new EntityNotFoundException($"{typeof(T).Name} with {keyName}='{key}' was not found.");
        //    }
        //}

        /// <summary>
        /// Validates that <paramref name="param"/> is positive number.
        /// </summary>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentException">If <paramref name="param"/> is not positive number.</exception>
        public static void ValidateArgumentPositive(long param, string paramName)
        {
            if (param <= 0)
            {
                throw new ArgumentException($"{paramName} should be positive.", paramName);
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is not <c>null</c>.
        /// </summary>
        ///
        /// <typeparam name="T">The type of the parameter, must be reference type.</typeparam>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentNullException">If <paramref name="param"/> is <c>null</c>.</exception>
        public static void ValidateArgumentNotNull<T>(T param, string paramName)
            where T : class
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is not <c>null</c> or empty.
        /// </summary>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentNullException">If <paramref name="param"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="param"/> is empty.</exception>
        public static void ValidateArgumentNotNullOrEmpty(string param, string paramName)
        {
            ValidateArgumentNotNull(param, paramName);
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is not <c>null</c> or empty.
        /// </summary>
        ///
        /// <typeparam name="T">Type of items in collection.</typeparam>
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentNullException">If <paramref name="param"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="param"/> is empty.</exception>
        public static void ValidateArgumentNotNullOrEmpty<T>(ICollection<T> param, string paramName)
        {
            ValidateArgumentNotNull(param, paramName);
            if (param.Count == 0)
            {
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
            }
        }

        /// <summary>
        /// Adds items to the source list.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items to add.</param>
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }

        /// <summary>
        /// Get end of the date for the given date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The end of the day.</returns>
        public static DateTime EOD(this DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }
    }
}
