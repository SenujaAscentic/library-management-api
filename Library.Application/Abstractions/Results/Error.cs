namespace Library.Application.Abstractions.Results
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        BusinessRule
    }

    public record Error(string Code, string Message, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Validation);
    }
}
