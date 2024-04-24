namespace API.Errors;

public class ApiException
{
    public ApiException(int statusCOde, string message, string details)
    {
        StatusCode = statusCOde;
        Message = message;
        Details = details;
    }

    public int StatusCode {get;set;}
    public string Message {get;set;}
    public string Details {get;set;}
}
