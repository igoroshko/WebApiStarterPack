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
    public class ApiErrorModel
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        public ApiErrorModel()
        {
        }

        public ApiErrorModel(ModelStateDictionary modelState)
        {
            IEnumerable<string> errors = modelState.Keys.SelectMany(key => modelState[key].Errors.Select(x => x.ErrorMessage));
            ErrorMessage = string.Join(Environment.NewLine, errors);
        }
    }
}
