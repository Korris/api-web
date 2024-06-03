using Mcsg.Lib.Common.Constants;

namespace Mcsg.Lib.Common.Exceptions
{
    public class AppUnauthorizedAccessException : BaseException
    {
        public AppUnauthorizedAccessException(string message) : base(ErrorCodes.UnauthorizeAccessCode, message)
        {
        }

        public AppUnauthorizedAccessException(string code, string message) : base(code, message)
        {
        }
    }
}