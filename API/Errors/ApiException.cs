namespace API.Errors;

public class ApiException
{
  private int _statusCode;
  private string _message;
  private string? _details;

  public ApiException(int statusCode, string message, string? details)
  {
      _statusCode = statusCode;
      _message = message;
      _details = details;
  }

  public int StatusCode { 
    get { return _statusCode; }
    set { _statusCode = value; }
  }

  public string Message { 
    get { return _message; }
    set { _message = value; }
  }

  public string? Details { 
    get { return _details; }
    set { _details = value; }
  }
}
