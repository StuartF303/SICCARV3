using BlueprintService.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using Siccar.Common.Exceptions;

namespace BlueprintService.V1.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var error = context.Error;

            if (error is DaprBlueprintRepositoryException exception)
            {
                return Problem(
                    title: exception.Message,
                    statusCode: (int)exception.Status,
                    detail: error.StackTrace
                    );
            }

            if (error is HttpStatusException httpsStatusException)
            {
                return Problem(
                    title: httpsStatusException.Message,
                    statusCode: (int)httpsStatusException.Status,
                    detail: error.StackTrace
                    );
            }

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var error = context.Error;

            if (error is DaprBlueprintRepositoryException exception)
            {
                return Problem(
                    title: exception.Message,
                    statusCode: (int)exception.Status
                    );
            }

            if (error is HttpStatusException httpsStatusException)
            {
                return Problem(
                    title: httpsStatusException.Message,
                    statusCode: (int)httpsStatusException.Status
                    );
            }

            return Problem();
        }

    }
}
