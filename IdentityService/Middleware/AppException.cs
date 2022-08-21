[Serializable]
public class AppException: Exception
{
    public int Status { get; set; } = 400;

    public AppException() {}
    public AppException(string message, int status) : base(message) { Status = status; }
    public AppException(string message) : base(message) {}

    public AppException(string message, Exception innerException) : base (message, innerException) {}    
}