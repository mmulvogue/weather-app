using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MM.WeatherService.Api.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    /// <summary>
    ///     Returns default problem details response for an application exception
    /// </summary>
    /// <returns></returns>
    [Route("error")]
    public IActionResult HandleError()
    {
        return Problem();
    }

    /// <summary>
    ///     Returns a problem details response with detailed exception info in title and message
    /// </summary>
    /// <param name="hostEnvironment"></param>
    /// <returns></returns>
    [Route("error-detailed")]
    public IActionResult HandleErrorWithDetails(
        [FromServices] IHostEnvironment hostEnvironment
    )
    {
        // Short circuit in case used in prod environment to avoid leaking
        // any implementation detail/private info from stack trace/error message
        if (!hostEnvironment.IsDevelopment()) return Problem();

        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        return Problem(
            exceptionHandlerFeature.Error.StackTrace,
            title: exceptionHandlerFeature.Error.Message
        );
    }
}
