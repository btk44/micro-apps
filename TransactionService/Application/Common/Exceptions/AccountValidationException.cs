namespace TransactionService.Application.Common.Exceptions;

[Serializable]
public class AccountValidationException: Exception
{
    public AccountValidationException() {}
    public AccountValidationException(string message) : base(message) {}

    public AccountValidationException(string message, Exception innerException) : base (message, innerException) {}    
}