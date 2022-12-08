using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MM.WeatherService.Api.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    /// <summary>
    ///     Returns default problem details for an application exception
    /// </summary>
    /// <returns></returns>
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError()
    {
        return Problem();
    }

    /// <summary>
    ///     Returns a problem details response with detailed exception info for development purposes
    /// </summary>
    /// <param name="hostEnvironment"></param>
    /// <returns></returns>
    [Route("/error-detailed")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleErrorWithDetails(
        [FromServices] IHostEnvironment hostEnvironment
    )
    {
        if (!hostEnvironment.IsDevelopment()) return Problem();

        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        return Problem(
            exceptionHandlerFeature.Error.StackTrace,
            title: exceptionHandlerFeature.Error.Message);
    }
}
