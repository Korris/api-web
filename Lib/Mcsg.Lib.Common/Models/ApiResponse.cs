namespace Mcsg.Lib.Common.Models;

public class ApiResponse
{
    public string Status { get; set; }
    public object Data { get; set; }
    public ApiErrorResponse Error { get; set; }
    public string Path { get; set; }
}
