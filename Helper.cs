/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Http.Controllers;
using log4net;
using Newtonsoft.Json;
using $safeprojectname$.Entities;
using $safeprojectname$.Exceptions;

namespace $safeprojectname$
{
    /// <summary>
    /// This class is the helper class for this project.
    /// </summary>
    ///
    /// <threadsafety>
    /// This class is immutable and thread safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    internal static class Helper
    {
        /// <summary>
        /// Represents the password mask.
        /// </summary>
        internal const string PasswordMask = "***";

        /// <summary>
        /// Represents the request property name for the current user.
        /// </summary>
        internal const string CurrentUserPropertyName = "CurrentUser";

        /// <summary>
        /// Represents the JSON serializer settings.
        /// </summary>
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = "MM/dd/yyyy HH:mm:ss",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        /// <summary>
        /// Parses string value to the enum.
        /// </summary>
        /// <remarks>Exceptions will be propagated.</remarks>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed enum value.</returns>
        internal static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Checks whether the given search criteria is <c>null</c> or incorrect.
        /// </summary>
        ///
        /// <param name="criteria">The search criteria to check.</param>
        ///
        /// <exception cref="ArgumentNullException">If the <paramref name="criteria"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the <paramref name="criteria"/> is incorrect,
        /// e.g. PageNumber is negative, or PageNumber is positive and PageSize is not positive.</exception>
        internal static void CheckSearchCriteria(BaseSearchCriteria criteria)
        {
            ValidateArgumentNotNull(criteria, nameof(criteria));

            if (criteria.PageNumber < 0)
            {
                throw new ArgumentException("Page number can't be negative.", nameof(criteria));
            }

            if (criteria.PageNumber > 0 && criteria.PageSize < 1)
            {
                throw new ArgumentException("Page size should be positive, if page number is positive.",
                    nameof(criteria));
            }
        }

        /// <summary>
        /// Checks whether the found entity is not null.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The found entity.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <exception cref="EntityNotFoundException">If <paramref name="entity"/> is null.</exception>
        internal static void CheckFoundEntity<T>(T entity, long entityId)
            where T : IdentifiableEntity
        {
            if (entity == null)
            {
                throw new EntityNotFoundException($"{typeof(T).Name} with Id='{entityId}' was not found.");
            }
        }

        /// <summary>
        /// Validates that <paramref name="property"/> is positive.
        /// </summary>
        ///
        /// <param name="property">The configuration property to validate.</param>
        /// <param name="propertyName">The name of the configuration property.</param>
        ///
        /// <exception cref="ConfigurationException">
        /// If <paramref name="property"/> is not positive.
        /// </exception>
        internal static void ValidateConfigPropertyPositive(int property, string propertyName)
        {
            if (property <= 0)
            {
                throw new ConfigurationException($"Configurable property {propertyName} must be positive.");
            }
        }

        /// <summary>
        /// Validates that <paramref name="property"/> is not <c>null</c>.
        /// </summary>
        ///
        /// <typeparam name="T">The type of the parameter, must be reference type.</typeparam>
        ///
        /// <param name="property">The configuration property to validate.</param>
        /// <param name="propertyName">The name of the configuration property.</param>
        ///
        /// <exception cref="ConfigurationException">
        /// If <paramref name="property"/> is <c>null</c>.
        /// </exception>
        internal static void ValidateConfigPropertyNotNull<T>(T property, string propertyName)
            where T : class
        {
            if (property == null)
            {
                throw new ConfigurationException($"Configurable property {propertyName} cannot be null.");
            }
        }

        /// <summary>
        /// Validates that <paramref name="property"/> is not <c>null</c> or empty.
        /// </summary>
        ///
        /// <param name="property">The configuration property to validate.</param>
        /// <param name="propertyName">The name of the configuration property.</param>
        ///
        /// <exception cref="ConfigurationException">
        /// If <paramref name="property"/> is <c>null</c> or empty.
        /// </exception>
        internal static void ValidateConfigPropertyNotNullOrEmpty(string property, string propertyName)
        {
            ValidateConfigPropertyNotNull(property, propertyName);
            if (string.IsNullOrWhiteSpace(property))
            {
                throw new ConfigurationException($"Configurable property {propertyName} cannot be empty.");
            }
        }

        /// <summary>
        /// Validates that <paramref name="property"/> is not <c>null</c> or empty collection.
        /// </summary>
        ///
        /// <typeparam name="T">Type of the entities in the collection.</typeparam>
        /// 
        /// <param name="property">The configuration property to validate.</param>
        /// <param name="propertyName">The name of the configuration property.</param>
        ///
        /// <exception cref="ConfigurationException">
        /// If <paramref name="property"/> is <c>null</c> or empty collection.
        /// </exception>
        internal static void ValidateConfigPropertyNotNullOrEmpty<T>(ICollection<T> property, string propertyName)
        {
            ValidateConfigPropertyNotNull(property, propertyName);
            if (property.Count == 0)
            {
                throw new ConfigurationException($"Configurable property {propertyName} cannot be empty.");
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is positive number.
        /// </summary>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentException">If <paramref name="param"/> is not positive number.</exception>
        internal static void ValidateArgumentPositive(long param, string paramName)
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
        internal static void ValidateArgumentNotNull<T>(T param, string paramName)
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
        internal static void ValidateArgumentNotNullOrEmpty(string param, string paramName)
        {
            ValidateArgumentNotNull(param, paramName);
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
            }
        }

        /// <summary>
        /// Validates that <paramref name="property"/> is not <c>null</c>.
        /// </summary>
        ///
        /// <typeparam name="T">The type of the parameter, must be reference type.</typeparam>
        ///
        /// <param name="property">The property to validate.</param>
        /// <param name="propertyName">The name of the parameter plus name of the property.</param>
        ///
        /// <exception cref="ArgumentException">If <paramref name="property"/> is <c>null</c>.</exception>
        internal static void ValidateArgumentPropertyNotNull<T>(T property, string propertyName)
            where T : class
        {
            if (property == null)
            {
                throw new ArgumentException($"{propertyName} cannot be null.", propertyName);
            }
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
                    throw new ServiceException($"'{colName}' is not a valid SortBy value.");
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
        /// Applies ThenBy extension method to the given Queryable source.
        /// </summary>
        /// <typeparam name="T">The type of queried entities.</typeparam>
        /// <param name="source">The Queryable source.</param>
        /// <param name="ordering">The order column name.</param>
        /// <returns>The new Queryable with applied ThenBy.</returns>
        /// <remarks>Thrown exceptions will be propagated.</remarks>
        internal static IQueryable<T> ThenBy<T>(this IQueryable<T> source, string ordering)
        {
            return OrderbyFromColumnName(source, "ThenBy", ordering);
        }

        /// <summary>
        /// Applies ThenByDescending extension method to the given Queryable source.
        /// </summary>
        /// <typeparam name="T">The type of queried entities.</typeparam>
        /// <param name="source">The Queryable source.</param>
        /// <param name="ordering">The order column name.</param>
        /// <returns>The new Queryable with applied ThenByDescending.</returns>
        /// <remarks>Thrown exceptions will be propagated.</remarks>
        internal static IQueryable<T> ThenByDescending<T>(this IQueryable<T> source,
            string ordering)
        {
            return OrderbyFromColumnName(source, "ThenByDescending", ordering);
        }

        /// <summary>
        /// Gets the full name of the action.
        /// </summary>
        /// <param name="context">The action context.</param>
        /// <param name="includeController">Determines whether 'Controller' suffix should be included.</param>
        /// <returns>The full name of the action.</returns>
        internal static string GetFullActionName(this HttpActionContext context, bool includeController = true)
        {
            return context.ActionDescriptor.ControllerDescriptor.ControllerName +
                (includeController ? "Controller" : string.Empty) + "." +
                context.ActionDescriptor.ActionName;
        }

        #region Logging helper methods

        /// <summary>
        /// Processes the specified action and wraps it with common logging and error handling logic.
        /// </summary>
        /// <remarks>
        /// If any exception is thrown, the <see cref="ArgumentException"/>, <see cref="ConfigurationException"/>,
        /// and <see cref="ServiceException"/> exceptions will be simply re-thrown.
        /// All other exceptions will be wrapped in <see cref="ServiceException"/> and thrown.
        /// </remarks>
        /// <param name="logger">The logger.</param>
        /// <param name="action">The action to process.</param>
        /// <param name="methodDescription">The short description of what the source method does.</param>
        /// <param name="callingMethod">The source method information.</param>
        /// <param name="parameters">The parameters for the source method.</param>
        internal static void Process(this ILog logger, Action action, string methodDescription,
            MethodBase callingMethod = null, params object[] parameters)
        {
            callingMethod = callingMethod ?? new StackTrace().GetFrame(1).GetMethod();
            string methodName = $"{callingMethod.DeclaringType.Name}.{callingMethod.Name}";

            Process(() =>
            {
                LogMethodEntry(logger, callingMethod, parameters);
                action();
                LogMethodExit(logger, methodName);
            },
            logger,
            methodDescription,
            methodName);
        }

        /// <summary>
        /// Processes the specified function and wraps it with common logging and error handling logic.
        /// </summary>
        /// <remarks>
        /// If any exception is thrown, the <see cref="ArgumentException"/>, <see cref="ConfigurationException"/>,
        /// and <see cref="ServiceException"/> exceptions will be simply re-thrown.
        /// All other exceptions will be wrapped in <see cref="ServiceException"/> and thrown.
        /// </remarks>
        /// <typeparam name="T">The type of the function return value.</typeparam>
        /// <param name="logger">The logger.</param>
        /// <param name="function">The function to process.</param>
        /// <param name="methodDescription">The short description of what the source method does.</param>
        /// <param name="callingMethod">The source method information.</param>
        /// <param name="parameters">The parameters for the source method.</param>
        /// <returns>The function result.</returns>
        internal static T Process<T>(this ILog logger, Func<T> function, string methodDescription,
            MethodBase callingMethod = null, params object[] parameters)
        {
            callingMethod = callingMethod ?? new StackTrace().GetFrame(1).GetMethod();
            string methodName = $"{callingMethod.DeclaringType.Name}.{callingMethod.Name}";

            T result = default(T);
            Process(() =>
            {
                LogMethodEntry(logger, callingMethod, parameters);
                result = function();
                LogMethodExit(logger, methodName, result);
            },
            logger,
            methodDescription,
            methodName);

            return result;
        }

        /// <summary>
        /// Processes the specified action and wraps it with common error handling logic.
        /// </summary>
        /// <remarks>
        /// If any exception is thrown, the <see cref="ArgumentException"/>, <see cref="ConfigurationException"/>,
        /// and <see cref="ServiceException"/> exceptions will be simply re-thrown.
        /// All other exceptions will be wrapped in <see cref="ServiceException"/> and thrown.
        /// </remarks>
        /// <param name="action">The action to process.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="methodDescription">The short description of what the source method does.</param>
        /// <param name="methodName">The full method name.</param>
        private static void Process(Action action, ILog logger, string methodDescription, string methodName)
        {
            Exception thrownException = null;
            try
            {
                action();
            }
            catch (ArgumentException ex)
            {
                thrownException = ex;
                throw;
            }
            catch (ConfigurationException ex)
            {
                thrownException = ex;
                throw;
            }
            catch (ServiceException ex)
            {
                thrownException = ex;
                throw;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error occurred while {methodDescription}.";
                thrownException = new ServiceException(errorMessage, ex);
                throw thrownException;
            }
            finally
            {
                if (thrownException != null)
                {
                    LogException(logger, methodName, thrownException);
                }
            }
        }

        /// <summary>
        /// Logs method entrance and input parameters with DEBUG level.
        /// </summary>
        /// <remarks>The internal exception may be thrown directly.</remarks>
        /// <param name="logger">The logger.</param>
        /// <param name="method">The method where the logging occurs.</param>
        /// <param name="parameters">The parameters used to format the message.</param>
        private static void LogMethodEntry(ILog logger, MethodBase method, params object[] parameters)
        {
            // Create a string format to display parameters
            var logFormat = new StringBuilder();
            var pis = method.GetParameters();
            string methodName = $"{method.DeclaringType.Name}.{method.Name}";
            if (pis.Length != parameters.Length)
            {
                throw new ArgumentException(
                    $"The number of provided parameters for method '{methodName}' is wrong.", nameof(parameters));
            }
            logFormat.AppendFormat("Entering method {0}", methodName).AppendLine();
            logFormat.AppendLine("Argument Values:");
            for (int i = 0; i < pis.Length; i++)
            {
                logFormat.Append("\t").Append(pis[i].Name).Append(": ");
                logFormat.AppendLine(GetObjectDescription(parameters[i]));
            }

            // log method entry and input parameters
            logger.Debug(logFormat.ToString());
        }

        /// <summary>
        /// Logs method exit with DEBUG level.
        /// </summary>
        ///
        /// <param name="logger">The logger.</param>
        /// <param name="methodName">The full method name.</param>
        private static void LogMethodExit(ILog logger, string methodName)
        {
            logger.DebugFormat("[Exiting method {0}]", methodName);
        }

        /// <summary>
        /// Logs method exit and output parameter with DEBUG level.
        /// </summary>
        ///
        /// <param name="logger">The logger.</param>
        /// <param name="methodName">The full method name.</param>
        /// <param name="returnValue">The method return value.</param>
        private static void LogMethodExit(ILog logger, string methodName, object returnValue)
        {
            LogMethodExit(logger, methodName);

            // log return value
            logger.DebugFormat("[Output parameter: {0}]", GetObjectDescription(returnValue));
        }

        /// <summary>
        /// Logs the given exception with ERROR level.
        /// </summary>
        ///
        /// <param name="exception">The exception to log.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="methodName">The full method name.</param>
        private static void LogException(ILog logger, string methodName, Exception exception)
        {
            // log exception
            logger.ErrorFormat("Error in method {0}.{1}Details:{1}{2}",
                methodName, Environment.NewLine, exception);
        }

        /// <summary>
        /// Gets JSON description of the object.
        /// </summary>
        ///
        /// <param name="obj">The object to describe.</param>
        /// <returns>The JSON description of the object.</returns>
        internal static string GetObjectDescription(object obj)
        {
            try
            {
                var stream = obj as System.IO.Stream;
                if (stream != null)
                {
                    return stream.Length.ToString();
                }

                return JsonConvert.SerializeObject(obj, SerializerSettings);
            }
            catch
            {
                return "[Can't express this value]";
            }
        }

        #endregion
    }
}