/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System.Collections.Generic;

namespace $safeprojectname$.Entities
{
    /// <summary>
    /// An entity class that represents search result.
    /// </summary>
    ///
    /// <typeparam name="T">The type of the items in the search result.</typeparam>
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
    public class SearchResult<T>
    {
        /// <summary>
        /// Gets or sets the total records count.
        /// </summary>
        /// <value>
        /// The total records count.
        /// </value>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Gets or sets the total pages count.
        /// </summary>
        /// <value>
        /// The total pages count.
        /// </value>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IList<T> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult{T}"/> class.
        /// </summary>
        public SearchResult()
        {
        }
    }
}
