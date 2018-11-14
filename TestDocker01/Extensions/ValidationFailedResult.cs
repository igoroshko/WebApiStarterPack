using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TestDocker01.Models;

namespace TestDocker01.Extensions
{
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState)
            : base(new ApiErrorModel(modelState))
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
