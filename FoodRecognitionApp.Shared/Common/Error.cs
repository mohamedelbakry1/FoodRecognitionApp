using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Common
{
    public sealed record Error(string Code, string Description, ErrorType ErrorType)
    {
        public static Error Failure(
            string code = "General.Failure",
            string description = "An unexpected error occurred.") =>
            new(code, description, ErrorType.Failure);

        public static Error Validation(
            string code = "General.Validation",
            string description = "One or more validation errors occurred.") =>
            new(code, description, ErrorType.Validation);

        public static Error NotFound(
            string code = "General.NotFound",
            string description = "The requested resource was not found.") =>
            new(code, description, ErrorType.NotFound);

        public static Error Conflict(
            string code = "General.Conflict",
            string description = "The request conflicts with an existing resource.") =>
            new(code, description, ErrorType.Conflict);

        public static Error Unauthorized(
            string code = "General.Unauthorized",
            string description = "Authentication is required to access this resource.") =>
            new(code, description, ErrorType.Unauthorized);
        public static Error Forbidden(
            string code = "General.Forbidden",
            string description = "You do not have permission to perform this action.") =>
            new(code, description, ErrorType.Forbidden);
        public static Error InvalidCredentials(
            string code = "General.InvalidCredentials",
            string description = "The email or password is incorrect.") =>
            new(code, description, ErrorType.InvalidCredentials);
    }
}
