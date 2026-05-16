using TenantFlow.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace TenantFlow.Api.Controllers
{
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult HandleFailure(Result result)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.Error),
                ErrorType.Unauthorized => Unauthorized(result.Error),
                ErrorType.Validation => BadRequest(result.Error),
                ErrorType.Conflict => Conflict(result.Error),
                ErrorType.ServerError => StatusCode(500, result.Error),
                ErrorType.None => StatusCode(500),
                _ => StatusCode(500)
            };
        }

        protected IActionResult HandleFailure<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.Error),
                ErrorType.Unauthorized => Unauthorized(result.Error),
                ErrorType.Validation => BadRequest(result.Error),
                ErrorType.Conflict => Conflict(result.Error),
                ErrorType.ServerError => StatusCode(500, result.Error),
                _ => StatusCode(500)
            };
        }
    }
}
