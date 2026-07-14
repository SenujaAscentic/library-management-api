namespace Library.Api.Contracts.Common;

public record ValidationError(
    string Field,
    string Message);