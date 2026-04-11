namespace BaseServerProject.Application.Common.Base;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }

    public static BaseResponse<T> Success(T data, string message = "Success", int statusCode = 200)
    {
        return new BaseResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static BaseResponse<T> Failure(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}