/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using log4net;
using Microsoft.Practices.Unity;
using $safeprojectname$.Entities;
using $safeprojectname$.Exceptions;

namespace $safeprojectname$.Services.Impl
{
    /// <summary>
    /// This abstract class is a base for all service implementations provided in this assembly.
    /// </summary>
    /// 
    /// <threadsafety>
    /// This class is mutable but effectively thread-safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public abstract class BaseService
    {
        /// <summary>
        /// Gets or sets the logger used for logging in this class.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// It is expected to be initialized by Unity and never changed after that.
        /// Should not be <c>null</c> after initialization.
        /// </para>
        /// It is used for logging in this class and its sub-classes.
        /// </remarks>
        ///
        /// <value>The logger.</value>
        [Dependency]
        public ILog Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        protected BaseService()
        {
        }

        /// <summary>
        /// Checks that all configuration properties were properly initialized.
        /// </summary>
        ///
        /// <exception cref="ConfigurationException">
        /// If any of required injection fields are not injected or have invalid values.
        /// </exception>
        public virtual void CheckConfiguration()
        {
            Helper.ValidateConfigPropertyNotNull(Logger, nameof(Logger));
        }

        /// <summary>
        /// Sets the page number and total pages for the given search result.
        /// </summary>
        /// <typeparam name="T">The type of the searched entities.</typeparam>
        /// <param name="result">The search result.</param>
        /// <param name="criteria">The search criteria.</param>
        protected static void SetSearchResultPageInfo<T>(SearchResult<T> result, BaseSearchCriteria criteria)
        {
            if (result.TotalRecords > 0)
            {
                result.TotalPages = criteria.PageNumber > 0
                    ? (result.TotalRecords - 1) / criteria.PageSize + 1
                    : 1;
            }
        }
    }
}
