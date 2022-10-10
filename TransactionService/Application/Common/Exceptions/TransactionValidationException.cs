namespace TransactionService.Application.Common.Exceptions;

[Serializable]
public class TransactionValidationException: Exception
{
    public TransactionValidationException() {}
    public TransactionValidationException(string message) : base(message) {}

    public TransactionValidationException(string message, Exception innerException) : base (message, innerException) {}    
}