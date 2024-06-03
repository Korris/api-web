namespace Mcsg.Lib.Common.Exceptions;

public class BaseException : Exception
{
    public string Code { get; set; }

    public BaseException(string code, string message) : base(message)
    {
        Code = code;
    }
}