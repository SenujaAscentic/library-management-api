namespace Library.Api.Contracts.Common;

public record ErrorResponse(
    int StatusCode,
    string Message,
    string TraceId
);