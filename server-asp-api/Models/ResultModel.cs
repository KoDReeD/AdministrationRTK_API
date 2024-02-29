using System.Net;

namespace server_asp_api.Models;

/// <summary>
/// Для методов контроллера
/// </summary>
public class ResultModel
{
    public HttpStatusCode StatusCode { get; set; }
    public object Data { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Для методов классов сервисов
/// </summary>
public class BoolMethodResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public static BoolMethodResult GetSuccessResult(string message)
    {
        return new BoolMethodResult()
        {
            IsSuccess = true,
            Message = message
        };;
    }
    
    public static BoolMethodResult GetBadRequest(string message)
    {
        return new BoolMethodResult()
        {
            IsSuccess = false,
            Message = message
        };;
    }
}

