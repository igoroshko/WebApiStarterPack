/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */

namespace $safeprojectname$.Entities
{
    /// <summary>
    /// An entity class that represents base search criteria.
    /// </summary>
    ///
    /// <remarks>
    /// Note that the properties are implemented without any validation.
    /// </remarks>
    ///
    /// <threadsafety>
    /// This class is mutable, so it is not thread-safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public abstract class BaseSearchCriteria
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the sort by property.
        /// </summary>
        /// <value>
        /// The sort by property.
        /// </value>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the type of the sort.
        /// </summary>
        /// <value>
        /// The type of the sort.
        /// </value>
        public SortType SortType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSearchCriteria"/> class.
        /// </summary>
        protected BaseSearchCriteria()
        {
        }
    }
}
