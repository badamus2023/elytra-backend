using System.Text.RegularExpressions;
using Drones.src.Api.Common.DTOs;

namespace Drones.src.Api.Common
{
    public static class ApiErrorMapper
    {
        private static readonly Dictionary<string, string> KnownCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["INVALID_CREDENTIALS"] = "Invalid email or password.",
            ["EMAIL_NOT_VERIFIED"] = "Please verify your email before signing in.",
            ["EMAIL_TAKEN"] = "An account with this email already exists.",
            ["INVALID_REFRESH_TOKEN"] = "Session expired. Please sign in again.",
            ["TOKEN_REVOKED"] = "Session expired. Please sign in again.",
            ["TOKEN_EXPIRED"] = "Session expired. Please sign in again.",
            ["ACCOUNT_DISABLED"] = "This account has been disabled.",
            ["DEFAULT_ROLE_NOT_FOUND"] = "Unable to complete registration. Please contact support.",
            ["INVALID_TOKEN"] = "This link is invalid or has already been used.",
            ["EMAIL_ALREADY_VERIFIED"] = "This email is already verified.",
            ["USER_NOT_FOUND"] = "User not found.",
            ["ORDER_NOT_FOUND"] = "Order not found.",
            ["RESTAURANT_NOT_FOUND"] = "Restaurant not found.",
            ["DELIVERY_POINT_NOT_FOUND"] = "Pickup point not found.",
            ["PAYMENT_NOT_FOUND"] = "Payment not found.",
        };

        private static readonly Regex CodePattern = new(@"^[A-Z][A-Z0-9_]+$", RegexOptions.Compiled);

        public static ApiErrorResponse FromException(Exception exception)
        {
            return exception switch
            {
                InvalidOperationException ex => FromCodeOrMessage(ex.Message),
                UnauthorizedAccessException ex => FromCodeOrMessage(ex.Message, defaultStatus401: true),
                KeyNotFoundException ex => new ApiErrorResponse
                {
                    Message = HumanizeMessage(ex.Message) ?? "Resource not found.",
                    Code = TryExtractCode(ex.Message),
                },
                _ => new ApiErrorResponse
                {
                    Message = "An unexpected error occurred. Please try again.",
                    Code = "INTERNAL_ERROR",
                },
            };
        }

        public static ApiErrorResponse FromValidationErrors(
            IDictionary<string, string[]> errors)
        {
            var normalized = errors.ToDictionary(
                pair => pair.Key,
                pair => pair.Value,
                StringComparer.OrdinalIgnoreCase);

            var firstMessage = normalized.Values
                .SelectMany(messages => messages)
                .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message));

            return new ApiErrorResponse
            {
                Message = firstMessage ?? "Please fix the validation errors and try again.",
                Code = "VALIDATION_FAILED",
                Errors = normalized,
            };
        }

        private static ApiErrorResponse FromCodeOrMessage(
            string rawMessage,
            bool defaultStatus401 = false)
        {
            var code = TryExtractCode(rawMessage);
            if (code != null && KnownCodes.TryGetValue(code, out var knownMessage))
            {
                return new ApiErrorResponse { Message = knownMessage, Code = code };
            }

            if (code != null)
            {
                return new ApiErrorResponse
                {
                    Message = HumanizeCode(code),
                    Code = code,
                };
            }

            if (!string.IsNullOrWhiteSpace(rawMessage))
            {
                return new ApiErrorResponse { Message = rawMessage.Trim() };
            }

            return new ApiErrorResponse
            {
                Message = defaultStatus401
                    ? "You are not authorized to perform this action."
                    : "Request failed.",
            };
        }

        private static string? TryExtractCode(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return null;

            var trimmed = message.Trim();
            return CodePattern.IsMatch(trimmed) ? trimmed : null;
        }

        private static string? HumanizeMessage(string message)
        {
            var code = TryExtractCode(message);
            return code != null ? HumanizeCode(code) : message.Trim();
        }

        private static string HumanizeCode(string code)
        {
            var words = code.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (words.Length == 0)
                return "Request failed.";

            var sentence = string.Join(' ', words.Select(word => word.ToLowerInvariant()));
            return char.ToUpperInvariant(sentence[0]) + sentence[1..] + ".";
        }
    }
}
