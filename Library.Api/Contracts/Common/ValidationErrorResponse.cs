namespace Library.Api.Contracts.Common;

public record ValidationErrorResponse(
    int StatusCode,
    string Message,
    List<ValidationError> Errors);