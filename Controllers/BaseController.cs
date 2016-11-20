/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;
using System.Web.Http;
using log4net;
using Microsoft.Practices.Unity;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;
using System.Threading;
using $safeprojectname$.Entities;
using $safeprojectname$.Exceptions;
using $safeprojectname$.Services;

namespace $safeprojectname$.Controllers
{
    /// <summary>
    /// The base class for all controllers.
    /// </summary>
    /// 
    /// <threadsafety>
    /// This class is mutable but effectively thread-safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public abstract class BaseController : ApiController
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
        /// Gets the current user from request properties.
        /// </summary>
        ///
        /// <value>
        /// The current user.
        /// </value>
        protected User CurrentUser
        {
            get
            {
                return (User)Request.Properties[Helper.CurrentUserPropertyName];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        protected BaseController()
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
        /// Downloads stream data as a file with given content type and file name.
        /// </summary>
        ///
        /// <param name="contentType">The content type.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="streamDataProvider">The data provider for the stream.</param>
        /// <returns>The HTTP response message with the binary data representing file.</returns>
        /// <remarks>All exceptions will be propagated.</remarks>
        protected static HttpResponseMessage DownloadFile(string contentType,
            string fileName, Action<Stream> streamDataProvider)
        {
            var stream = new MemoryStream();
            streamDataProvider(stream);
            stream.Position = 0;
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;
            return result;
        }

        /// <summary>
        /// Wraps given action with transaction scope.
        /// </summary>
        /// <remarks>All exceptions will be propagated to caller method.</remarks>
        /// <param name="action">The action to execute.</param>
        protected static void WithTransaction(Action action)
        {
            using (var transactionScope = new TransactionScope())
            {
                action();
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Extracts the attachment from the request content.
        /// </summary>
        /// 
        /// <returns>The extracted attachment.</returns>
        /// <remarks>All exceptions will be propagated.</remarks>
        protected Attachment ExtractAttachment()
        {
            // guarantees separate thread otherwise request can enter in deadlock when multiple
            // request are sent because content.ReadAsMultipartAsync may launch task on same thread
            MultipartMemoryStreamProvider provider = null;
            Task.Factory.StartNew(() => provider = Request.Content.ReadAsMultipartAsync().Result,
                CancellationToken.None,
                TaskCreationOptions.LongRunning, //guarantees separate thread
                TaskScheduler.Default).Wait();

            // get file content with name 'attachment'
            HttpContent content = provider.Contents.FirstOrDefault(x =>
                x.Headers.ContentDisposition.Name.Replace("\"", string.Empty).Normalize().ToLower() == "attachment");

            if (content == null)
            {
                return null;
            }

            var attachment = new Attachment
            {
                FileName = Guid.NewGuid().ToString(),
                MimeType = content.Headers.ContentType.MediaType
            };

            // save file to temporary folder
            string tempFolder = Path.GetTempPath();
            byte[] bytes = content.ReadAsByteArrayAsync().Result;
            using (var fs = new FileStream(Path.Combine(tempFolder, attachment.FileName), FileMode.Create))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(bytes);
                }
            }
            return attachment;
        }
    }
}
