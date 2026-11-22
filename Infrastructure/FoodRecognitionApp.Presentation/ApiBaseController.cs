using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Shared.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FoodRecognitionApp.Presentation
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected ActionResult ToActionResult(Result result)
        {
            if (result.IsSuccess)
                return new OkResult();
            else
                return ToProblem(result.Errors);
        }

        protected ActionResult<T> ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);
            else
                return ToProblem(result.Errors);
        }

        private ObjectResult ToProblem(IReadOnlyList<Error> errors)
        {
            if (errors is null || errors.Count == 0)
            {
                throw new InvalidOperationException("Cannot generate a problem response from an empty errors list.");
            }
            var firstError = errors[0];
            var statusCode = firstError.ErrorType switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Failure => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status400BadRequest
            };
            var problem = new ProblemDetails
            {
                Title = firstError.Code,
                Detail = firstError.Description,
                Status = statusCode,
                Instance = HttpContext.Request.Path.Value,
                Extensions = { ["errors"] = errors }
            };
            return new ObjectResult(problem) { StatusCode = statusCode };
        }

        protected Guid GetUserIdFromToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var id) ? id : throw new UnAuthorizedException("You are not authorized to perform this action.");
        }
    }
}
