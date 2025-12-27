namespace SmartInventory.BLL.Model;

public class Result<T>
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
    public T? Data { get; set; }

    public Result(bool success, T? data, string? error)
    {
        Success = success;
        Data = data;
        Error = error ?? string.Empty;
    }

    public static Result<T> SuccessResult(T data)
    {
        return new Result<T>(true, data, string.Empty);
    }

    public static Result<T> FaileResult(string error)
    {
        return new Result<T>(false, default, error);
    }
}
