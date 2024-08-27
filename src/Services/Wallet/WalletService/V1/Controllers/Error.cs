using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WalletService.Core;
using WalletService.Exceptions;

namespace WalletService.V1.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment, [FromServices] ILoggerFactory loggerFactory)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var error = context.Error;

            if (error is WalletException repositoryStatusException)
            {
                var walletLogger = loggerFactory.CreateLogger<WalletException>();
                walletLogger.LogError("WalletException: {msg}", error.Message);

                return Problem(
                    title: repositoryStatusException.Message,
                    statusCode: (int)repositoryStatusException.Status,
                    detail: error.StackTrace
                    );
            }

            if (error is HttpStatusException httpsStatusException)
            {
                var httpError = loggerFactory.CreateLogger<WalletException>();
                httpError.LogError("WalletException: {msg}", error.Message);

                return Problem(
                    title: httpsStatusException.Message,
                    statusCode: (int)httpsStatusException.Status,
                    detail: error.StackTrace
                    );
            }

            var errorLogger = loggerFactory.CreateLogger<ErrorController>();
            errorLogger.LogError("WalletException: {msg}", error.Message);
            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var error = context.Error;

            if (error is WalletException repositoryStatusException)
            {
                return Problem(
                    title: repositoryStatusException.Message,
                    statusCode: (int)repositoryStatusException.Status,
                    detail: error.StackTrace
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
