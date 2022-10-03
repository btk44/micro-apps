namespace TransactionService.Application.Common.Exceptions;

[Serializable]
public class CategoryValidationException: Exception
{
    public CategoryValidationException() {}
    public CategoryValidationException(string message) : base(message) {}

    public CategoryValidationException(string message, Exception innerException) : base (message, innerException) {}    
}