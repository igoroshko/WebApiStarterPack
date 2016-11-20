/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using log4net;
using log4net.Config;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using $safeprojectname$.Filters;
using Unity.WebApi;

namespace $safeprojectname$
{
    /// <summary>
    /// This class is responsible for configuring Web API infrastructure.
    /// </summary>
    ///
    /// <threadsafety>
    /// This class is not thread safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// Custom response handler to return 200 for successful result.
        /// </summary>
        public class ResponseHandler : DelegatingHandler
        {
            /// <summary>
            /// Set status as 200 for successful result.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>The HTTP response message task.</returns>
            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = base.SendAsync(request, cancellationToken);
                response.Result.StatusCode = response.Result.IsSuccessStatusCode
                    ? HttpStatusCode.OK : response.Result.StatusCode;
                return response;
            }
        }

        /// <summary>
        /// When the application is started, registers all required filters, routers, dependency resolver, etc.
        /// </summary>
        /// <remarks>
        /// Exceptions will be propagated to caller method.
        /// </remarks>
        protected void Application_Start()
        {
            // configure logger
            XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger("default");

            try
            {
                // set up Unity
                var unitySection = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                var unityContainer = new UnityContainer().LoadConfiguration(unitySection)
                    .RegisterInstance(typeof(ILog), logger);

                GlobalConfiguration.Configure(config =>
                {
                    config.MapHttpAttributeRoutes();

                    // set unity dependency resolver
                    config.DependencyResolver = new UnityDependencyResolver(unityContainer);

                    // please note that the order is important, Logging filter should come before Authorization filter
                    config.Filters.Add(unityContainer.Resolve<ExceptionFilter>());
                    config.Filters.Add(unityContainer.Resolve<LoggingActionFilter>());
                    config.Filters.Add(unityContainer.Resolve<AuthorizationFilter>());

                    // set camel case serialization
                    config.Formatters.JsonFormatter.SerializerSettings.ContractResolver
                        = new CamelCasePropertyNamesContractResolver();

                    // string enum converter
                    config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                        new Newtonsoft.Json.Converters.StringEnumConverter());

                    // ignore reference loops
                    config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
                        = ReferenceLoopHandling.Ignore;

                    config.MessageHandlers.Add(new ResponseHandler());
                });
            }
            catch (Exception ex)
            {
                logger.Error("Error while starting the application.", ex);
                throw;
            }
        }
    }
}
