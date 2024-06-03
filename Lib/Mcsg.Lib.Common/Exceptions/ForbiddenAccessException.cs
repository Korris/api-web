using Mcsg.Lib.Common.Constants;

namespace Mcsg.Lib.Common.Exceptions
{
    public class ForbiddenAccessException : BaseException
    {
        public ForbiddenAccessException(string message) : base(ErrorCodes.ForbiddenAccessCode, message)
        {
        }

        public ForbiddenAccessException(string code, string message) : base(code, message)
        {
        }
    }
}