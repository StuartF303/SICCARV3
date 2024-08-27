using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Siccar.Common.Exceptions;

namespace Siccar.Registers.RegisterService.V1.Controllers
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
