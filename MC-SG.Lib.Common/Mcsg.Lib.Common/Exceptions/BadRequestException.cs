using Mcsg.Lib.Common.Constants;

namespace Mcsg.Lib.Common.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(ErrorCodes.BadRequestCode, message)
        {
        }

        public BadRequestException(string code, string message) : base(code, message)
        {
        }
    }
}