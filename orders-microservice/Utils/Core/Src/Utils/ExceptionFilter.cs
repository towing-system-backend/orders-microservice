using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Application.Core
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var result = new ObjectResult(new { error = exception.Message })
            {
                StatusCode = exception is InvalidOperationException ? 400 : 500
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}