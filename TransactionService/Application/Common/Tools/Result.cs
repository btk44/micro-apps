namespace TransactionService.Application.Common.Tools;

public readonly struct Result<V> {
    internal readonly bool IsSuccess;
    internal readonly V Value;
    internal readonly Exception Exception;

    public Result(V value)
    {
        Value = value;
        IsSuccess = true;
        Exception = null;
    }

    public Result(Exception exception)
    {
        Exception = exception;
        IsSuccess = false;
        Value = default(V);
    }

    public T Match<T>(Func<V, T> valueFn, Func<Exception, T> exceptionFn){
        return IsSuccess ? valueFn(Value) : exceptionFn(Exception);
    }
}