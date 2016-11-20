/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */

namespace $safeprojectname$.Entities
{
    /// <summary>
    /// An entity class that represents the action permission.
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
    public class ActionPermission : IdentifiableEntity
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPermission"/> class.
        /// </summary>
        public ActionPermission()
        {
        }
    }
}
