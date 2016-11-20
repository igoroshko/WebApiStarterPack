/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using Microsoft.Practices.Unity;
using $safeprojectname$.Services;
using $safeprojectname$.Exceptions;
using $safeprojectname$.Models;
using $safeprojectname$.Entities;

namespace $safeprojectname$.Controllers
{
    /// <summary>
    /// This controller exposes security related operations.
    /// </summary>
    /// 
    /// <threadsafety>
    /// This class is mutable but effectively thread-safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    [RoutePrefix("api/v1")]
    public class SecurityController : BaseController
    {
        /// <summary>
        /// Gets or sets the <see cref="ISecurityService"/> dependency.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// It is expected to be initialized by Unity and never changed after that.
        /// Should not be <c>null</c> after initialization.
        /// </para>
        /// It is used for security operations.
        /// </remarks>
        ///
        /// <value>The <see cref="ISecurityService"/> dependency.</value>
        [Dependency]
        public ISecurityService SecurityService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityController"/> class.
        /// </summary>
        public SecurityController()
        {
        }

        /// <summary>
        /// Checks that all configuration properties were properly initialized.
        /// </summary>
        ///
        /// <exception cref="ConfigurationException">
        /// If any of required injection fields are not injected or have invalid values.
        /// </exception>
        public override void CheckConfiguration()
        {
            base.CheckConfiguration();
            Helper.ValidateConfigPropertyNotNull(SecurityService, nameof(SecurityService));
        }

        /// <summary>
        /// Logs in user with the given login credentials.
        /// </summary>
        ///
        /// <param name="login">The login credentials.</param>
        /// <returns>The login result entity.</returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="login"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="HttpResponseException">
        /// If cannot authenticate user with the given credentials.</exception>
        /// <remarks>All exceptions from back-end services will be propagated.</remarks>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public LoginResult Login(LoginRequest login)
        {
            Helper.ValidateArgumentNotNull(login, nameof(login));

            User user = SecurityService.Authenticate(login.Username, login.Password);
            if (user == null)
            {
                var message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent("Cannot authenticate user with the given credentials.");
                throw new HttpResponseException(message);
            }

            Token token = SecurityService.GenerateToken(user);
            return new LoginResult { Token = token, User = user };
        }

        /// <summary>
        /// Revokes the current user token.
        /// </summary>
        ///
        /// <remarks>All exceptions from back-end services will be propagated.</remarks>
        [HttpPost]
        [Route("revokeToken")]
        public void RevokeToken()
        {
            string actionName = Helper.GetFullActionName(ActionContext, includeController: false);

            string token = ActionContext.Request.Headers.Authorization.Parameter;
            SecurityService.RevokeToken(token);
        }
    }
}
