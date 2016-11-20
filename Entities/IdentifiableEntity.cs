/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */

namespace $safeprojectname$.Entities
{
    /// <summary>
    /// Represents the base class for all identifiable entities.
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
    public abstract class IdentifiableEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableEntity"/> class.
        /// </summary>
        protected IdentifiableEntity()
        {
        }
    }
}
