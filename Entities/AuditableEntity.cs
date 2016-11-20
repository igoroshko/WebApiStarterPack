/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;

namespace $safeprojectname$.Entities
{
    /// <summary>
    /// Represents the base class for all auditable entities.
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
    public abstract class AuditableEntity : IdentifiableEntity
    {
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created time.
        /// </summary>
        /// <value>
        /// The created time.
        /// </value>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the last updated by.
        /// </summary>
        /// <value>
        /// The last updated by.
        /// </value>
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the last updated time.
        /// </summary>
        /// <value>
        /// The last updated time.
        /// </value>
        public DateTime LastUpdatedTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditableEntity"/> class.
        /// </summary>
        protected AuditableEntity()
        {
        }
    }
}
