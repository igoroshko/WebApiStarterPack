/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using log4net;
using Microsoft.Practices.Unity;
using $safeprojectname$.Exceptions;

namespace $safeprojectname$.Filters
{
    /// <summary>
    /// This filter translates exceptions to HTTP status codes.
    /// </summary>
    /// 
    /// <threadsafety>
    /// This class is mutable but effectively thread-safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public class ExceptionFilter : ExceptionFilterAttribute
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
        /// Initializes a new instance of the <see cref="ExceptionFilter"/> class.
        /// </summary>
        public ExceptionFilter()
        {
        }

        /// <summary>
        /// Checks that all configuration properties were properly initialized.
        /// </summary>
        ///
        /// <exception cref="ConfigurationException">
        /// If any of required injection fields are not injected or have invalid values.
        /// </exception>
        public void CheckConfiguration()
        {
            Helper.ValidateConfigPropertyNotNull(Logger, nameof(Logger));
        }

        /// <summary>
        /// This method is executed when exception is thrown from the action. This method is
        /// responsible for logging the exception and setting appropriate HTTP StatusCode and
        /// error message to the result, in case the it is a AJAX request.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnException(HttpActionExecutedContext filterContext)
        {
            // Log exception
            string actionName = filterContext.ActionContext.GetFullActionName();
            Logger.Error($"[Error in action '{actionName}'. Details:{Environment.NewLine}{filterContext.Exception}]");

            HttpStatusCode statusCode;
            if (filterContext.Exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (filterContext.Exception is EntityNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            filterContext.Response = new HttpResponseMessage(statusCode);
            filterContext.Response.Content = new StringContent(filterContext.Exception.Message);
        }
    }
}
