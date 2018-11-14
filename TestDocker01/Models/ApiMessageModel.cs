/*
 * Copyright (c) 2018, TopCoder, Inc. All rights reserved.
 */

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestDocker01.Models
{
    /// <summary>
    /// An API error DTO.
    /// </summary>
    public class ApiMessageModel
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string Message { get; set; }

        public ApiMessageModel()
        {
        }
    }
}
