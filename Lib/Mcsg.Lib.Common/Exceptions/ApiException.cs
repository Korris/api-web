using Mcsg.Lib.Common.Constants;

namespace Mcsg.Lib.Common.Exceptions
{
    public class ApiException : BaseException
    {
        public ApiException(string message) : base(ErrorCodes.ApiErrorCode, message)
        {
        }

        public ApiException(string code, string message) : base(code, message)
        {
        }
    }
}